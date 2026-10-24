[CmdletBinding()]
param(
    [string]$EnvFile
)

$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($EnvFile)) {
    $EnvFile = Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Path) '..\.env.local'
}

function Import-DotEnvIfPresent {
    param([string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) { return }

    foreach ($line in Get-Content -LiteralPath $Path) {
        if ($line -match '^\s*(#|$)') { continue }
        if ($line -notmatch '^\s*([^=\s]+)\s*=\s*(.*)\s*$') {
            throw "Linea invalida en $Path. Use NOMBRE=valor."
        }

        $name = $Matches[1]
        $value = $Matches[2].Trim('"')
        if ([string]::IsNullOrWhiteSpace([Environment]::GetEnvironmentVariable($name))) {
            [Environment]::SetEnvironmentVariable($name, $value, 'Process')
        }
    }
}

function Require-EnvironmentVariable {
    param([string]$Name)

    $value = [Environment]::GetEnvironmentVariable($Name)
    if ([string]::IsNullOrWhiteSpace($value)) {
        throw "Falta la variable requerida $Name. Revise .env.example."
    }
    return $value
}

function Assert-StatusCode {
    param([int]$Actual, [int]$Expected, [string]$Operation)

    if ($Actual -ne $Expected) {
        throw "$Operation devolvio HTTP $Actual; se esperaba $Expected."
    }
}

function Invoke-StatusRequest {
    param(
        [string]$Uri,
        [hashtable]$Headers
    )

    $request = @{
        Method = 'Get'
        Uri = $Uri
        UseBasicParsing = $true
        ErrorAction = 'Stop'
    }
    if ($null -ne $Headers) { $request.Headers = $Headers }

    try {
        $response = Invoke-WebRequest @request
        return [PSCustomObject]@{ StatusCode = [int]$response.StatusCode }
    }
    catch [System.Net.WebException] {
        if ($null -eq $_.Exception.Response) { throw }
        return [PSCustomObject]@{ StatusCode = [int]$_.Exception.Response.StatusCode }
    }
}

Import-DotEnvIfPresent -Path $EnvFile

$apiUrl = (Require-EnvironmentVariable 'ELPRADO_TEST_API_URL').TrimEnd('/')
$username = Require-EnvironmentVariable 'ELPRADO_TEST_USERNAME'
$password = Require-EnvironmentVariable 'ELPRADO_TEST_PASSWORD'
$periodo = Require-EnvironmentVariable 'ELPRADO_STATS_TEST_PERIOD'
$desde = Require-EnvironmentVariable 'ELPRADO_STATS_TEST_FROM'
$hasta = Require-EnvironmentVariable 'ELPRADO_STATS_TEST_TO'
$periodoFaltante = [Environment]::GetEnvironmentVariable('ELPRADO_STATS_MISSING_PERIOD')
if ([string]::IsNullOrWhiteSpace($periodoFaltante)) { $periodoFaltante = '2024-01' }
$thresholdMs = [int](Require-EnvironmentVariable 'ELPRADO_STATS_LATENCY_THRESHOLD_MS')

$login = Invoke-RestMethod -Method Post -Uri "$apiUrl/api/Login/Usuario" -ContentType 'application/json' -Body (@{
    alias = $username
    clave = $password
} | ConvertTo-Json -Compress)

$token = $login.data.token
if ([string]::IsNullOrWhiteSpace($token)) {
    throw 'El login no devolvio un JWT.'
}

$headers = @{ Authorization = "Bearer $token" }
$summaryUrl = "$apiUrl/api/estadisticas/parcelas/resumen?periodo=$periodo&producto=todos"
$evolutionUrl = "$apiUrl/api/estadisticas/parcelas/evolucion?desde=$desde&hasta=$hasta&producto=todos"

$unauthorized = Invoke-StatusRequest -Uri $summaryUrl
Assert-StatusCode -Actual $unauthorized.StatusCode -Expected 401 -Operation 'Resumen sin JWT'

$invalid = Invoke-StatusRequest -Headers $headers -Uri "$apiUrl/api/estadisticas/parcelas/resumen?periodo=invalido"
Assert-StatusCode -Actual $invalid.StatusCode -Expected 400 -Operation 'Resumen con periodo invalido'

$invalidProduct = Invoke-StatusRequest -Headers $headers -Uri "$apiUrl/api/estadisticas/parcelas/resumen?periodo=$periodo&producto=otro"
Assert-StatusCode -Actual $invalidProduct.StatusCode -Expected 400 -Operation 'Resumen con producto invalido'

$summaryElapsed = Measure-Command {
    $summary = Invoke-RestMethod -Method Get -Headers $headers -Uri $summaryUrl
}
$evolutionElapsed = Measure-Command {
    $evolution = Invoke-RestMethod -Method Get -Headers $headers -Uri $evolutionUrl
}
$dpc = Invoke-RestMethod -Method Get -Headers $headers -Uri "$apiUrl/api/estadisticas/parcelas/resumen?periodo=$periodo&producto=dpc"
$flores = Invoke-RestMethod -Method Get -Headers $headers -Uri "$apiUrl/api/estadisticas/parcelas/resumen?periodo=$periodo&producto=flores-siempre"
$missing = Invoke-RestMethod -Method Get -Headers $headers -Uri "$apiUrl/api/estadisticas/parcelas/resumen?periodo=$periodoFaltante"

if (-not $summary.data.datosDisponibles) {
    throw "El resumen de $periodo no informa datos disponibles."
}
if ($summary.data.kpi -eq $null) {
    throw 'El resumen disponible no incluye KPI.'
}
if ($evolution.data.observaciones.Count -eq 0) {
    throw 'La evolucion no incluye observaciones disponibles.'
}
if (-not $dpc.data.datosDisponibles -or -not $flores.data.datosDisponibles) {
    throw 'Los filtros dpc y flores-siempre deben devolver datos disponibles.'
}
if ($missing.data.datosDisponibles -or $null -ne $missing.data.kpi) {
    throw "El periodo faltante $periodoFaltante debe responder datosDisponibles=false y kpi=null."
}

$serialized = ($summary.data, $evolution.data | ConvertTo-Json -Depth 10)
if ($serialized -match '(?i)cod(parcela|propuesta)') {
    throw 'La respuesta publico identificadores individuales de parcelas o propuestas.'
}

$summaryMs = [Math]::Round($summaryElapsed.TotalMilliseconds, 2)
$evolutionMs = [Math]::Round($evolutionElapsed.TotalMilliseconds, 2)
if ($summaryMs -gt $thresholdMs -or $evolutionMs -gt $thresholdMs) {
    throw "Latencia excedida: resumen $summaryMs ms, evolucion $evolutionMs ms; umbral $thresholdMs ms."
}

[PSCustomObject]@{
    ResumenMs = $summaryMs
    EvolucionMs = $evolutionMs
    UmbralMs = $thresholdMs
    Periodo = $periodo
    Observaciones = $evolution.data.observaciones.Count
    PeriodosFaltantes = $evolution.data.periodosFaltantes.Count
} | Format-List
