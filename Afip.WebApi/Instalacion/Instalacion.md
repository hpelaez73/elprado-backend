# Instalación y Despliegue de ApiWsfe en Linux

## Descripción

`ApiWsfe` es un servicio ASP.NET Core que actúa como intermediario entre aplicaciones clientes y los Web Services de AFIP.

La aplicación:

* Se conecta a Firebird.
* Gestiona certificados AFIP.
* Consume los servicios SOAP de AFIP.
* Expone una API REST para ser consumida desde aplicaciones Delphi.

---

# Requisitos

## Sistema Operativo

* Debian 10 o superior
* Ubuntu 20.04 o superior

## Firebird

Firebird debe estar instalado y funcionando.

Verificar:

```bash
systemctl status firebird
```

o

```bash
netstat -tlnp | grep 3050
```

Ejemplo:

```text
tcp 0 0 192.168.1.1:3050 0.0.0.0:* LISTEN 420/firebird
```

---

# Publicación desde Visual Studio

Agregar al archivo `.csproj`:

```xml
<PublishSingleFile>true</PublishSingleFile>
<SelfContained>true</SelfContained>
<InvariantGlobalization>true</InvariantGlobalization>
```

Publicar para Linux x64:

```bash
dotnet publish -c Release -r linux-x64 --self-contained true
```

La carpeta publicada contendrá:

```text
Afip.WebApi
appsettings.json
```

---

# Copia al servidor

Crear carpeta:

```bash
mkdir -p /opt/apiwsfe
```

Copiar archivos publicados:

```bash
scp -r publish/* root@192.168.1.1:/opt/apiwsfe/
```

Dar permisos de ejecución:

```bash
chmod +x /opt/apiwsfe/Afip.WebApi
```

---

# Usuario del servicio

Crear usuario dedicado:

```bash
useradd -r -s /usr/sbin/nologin -d /opt/apiwsfe apiwsfe
```

Asignar permisos:

```bash
chown -R apiwsfe:apiwsfe /opt/apiwsfe
```

---

# Configuración

## appsettings.json

Ejemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "DataSource=192.168.1.1;Database=/var/data/elprado.fdb;Port=3050;User=sysdba;Password=cinfor;Dialect=3;Charset=ISO8859_1;Connection lifetime=10;Pooling=true;MinPoolSize=0;MaxPoolSize=50"
  },

  "rutaCertificado": "/opt/apiwsfe/certificados",

  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:5000"
      }
    }
  }
}
```

### Importante

En este servidor Firebird escucha únicamente en:

```text
192.168.1.1:3050
```

Por lo tanto NO debe utilizarse:

```text
DataSource=127.0.0.1
```

porque genera:

```text
Connection refused 127.0.0.1:3050
```

---

# Certificados AFIP

Crear carpeta:

```bash
mkdir -p /opt/apiwsfe/certificados
```

Copiar:

```text
certificado.crt
private.key
certificado.pfx
```

Asignar permisos:

```bash
chown -R apiwsfe:apiwsfe /opt/apiwsfe/certificados
chmod -R 700 /opt/apiwsfe/certificados
```

---

# Instalación como servicio Linux

Crear archivo:

```bash
nano /etc/systemd/system/apiwsfe.service
```

Contenido:

```ini
[Unit]
Description=Servidor AFIP
After=network.target firebird.service

[Service]
WorkingDirectory=/opt/apiwsfe
ExecStart=/opt/apiwsfe/Afip.WebApi

User=apiwsfe
Group=apiwsfe

Restart=always
RestartSec=5

[Install]
WantedBy=multi-user.target
```

---

# Registrar servicio

Recargar configuración:

```bash
systemctl daemon-reload
```

Habilitar inicio automático:

```bash
systemctl enable apiwsfe
```

Iniciar:

```bash
systemctl start apiwsfe
```

---

# Verificar estado

```bash
systemctl status apiwsfe
```

Resultado esperado:

```text
Active: active (running)
```

---

# Reiniciar servicio

```bash
systemctl restart apiwsfe
```

---

# Detener servicio

```bash
systemctl stop apiwsfe
```

---

# Ver logs

Últimos 100 registros:

```bash
journalctl -u apiwsfe -n 100 --no-pager
```

Monitoreo en tiempo real:

```bash
journalctl -u apiwsfe -f
```

Desde una fecha determinada:

```bash
journalctl -u apiwsfe --since "2026-06-18 20:00:00"
```

---

# Verificar puerto HTTP

```bash
ss -tlnp | grep 5000
```

Resultado esperado:

```text
LISTEN 0 512 0.0.0.0:5000
```

---

# Verificar puerto Firebird

```bash
ss -tlnp | grep 3050
```

Resultado esperado:

```text
LISTEN 0 50 192.168.1.1:3050
```

---

# Pruebas

## Estado del servicio

```bash
curl http://localhost:5000/api/wsfe/estado-servicio
```

o desde otra PC:

```bash
curl http://192.168.1.1:5000/api/wsfe/estado-servicio
```

---

## Último comprobante

```bash
curl http://192.168.1.1:5000/api/wsfe/ultimo-comprobante/1/102
```

---

# Diagnóstico rápido

## Puerto 5000 ocupado

```text
Failed to bind to address http://0.0.0.0:5000
```

Ver proceso:

```bash
ss -tlnp | grep 5000
```

---

## Error Firebird

```text
Connection refused 127.0.0.1:3050
```

Verificar:

```bash
ss -tlnp | grep 3050
```

Confirmar que el DataSource coincida con la IP donde escucha Firebird.

---

## Verificar que la API esté accesible

Desde otra máquina:

```bash
curl http://192.168.1.1:5000/api/wsfe/estado-servicio
```

Si responde correctamente, el servicio está operativo.
