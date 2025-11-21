### Linux TLS cipher mismatch with FTPS

This repository provides a minimal, reproducible setup demonstrating a TLS interoperability issue that affects FluentFTP when running on Linux. The issue occurs when connecting to FTPS servers that only support RSA key-exchange TLS 1.2 cipher suites (no ECDHE/DHE).

On Linux, .NET’s SslStream does not offer RSA key-exchange cipher suites by default. As a result, FluentFTP cannot complete the TLS handshake on these servers unless it exposes SslClientAuthenticationOptions and allows custom CipherSuitesPolicy.

This repository demonstrates:

The FTPS server works with OpenSSL.

The FTPS server fails with .NET and FluentFTP if the cipher is not configured in FluentFTP.

The connection succeeds after specifying RSA cipher suites with CipherSuitesPolicy.

**Microsoft documents this behavior**: https://learn.microsoft.com/en-us/dotnet/core/compatibility/cryptography/5.0/default-cipher-suites-for-tls-on-linux

#### 1. FTPS Legacy Server

The included Docker image runs vsftpd and enforces RSA-only TLS 1.2 cipher suites to reproduce the affected environment.


```bash
docker compose up --build ftps-legacy
```
#### 2. Verify via OpenSSL - It should succeed

```bash
openssl s_client -connect localhost:2121 -starttls ftp -tls1_2
```

#### 3. .NET SslStream Test (Linux, .NET Version > 5) - It should fail

Run the FTP.Legacy.Demo .NET Project. 

If these lines are commented it should fail with sslv3 alert handshake fail:  

```
CipherSuitesPolicy = new CipherSuitesPolicy(new[]
 {
    TlsCipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384
 })
```

If these lines are uncommented the connection should pass.


Due to this behaviour, FluentFTP also fails with the same error: 

```
SSL Handshake failed with OpenSSL error - SSL_ERROR_SSL
error:0A000410:SSL routines::sslv3 alert handshake failure
```