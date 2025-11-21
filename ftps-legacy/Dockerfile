FROM ubuntu:22.04

RUN apt-get update && \
    DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
        vsftpd openssl ca-certificates && \
    rm -rf /var/lib/apt/lists/*

RUN useradd -m -d /home/ftpuser -s /bin/bash ftpuser && \
    echo "ftpuser:ftppass" | chpasswd && \
    mkdir -p /home/ftpuser/ftp && \
    chown -R ftpuser:ftpuser /home/ftpuser/ftp

RUN mkdir -p /var/run/vsftpd/empty && \
    chmod 755 /var/run/vsftpd/empty

RUN mkdir -p /etc/ssl/private && \
    openssl req -x509 -nodes -days 365 \
        -newkey rsa:2048 \
        -keyout /etc/ssl/private/vsftpd.key \
        -out /etc/ssl/private/vsftpd.crt \
        -subj "/CN=localhost" && \
    cat /etc/ssl/private/vsftpd.key /etc/ssl/private/vsftpd.crt > /etc/ssl/private/vsftpd.pem && \
    chmod 600 /etc/ssl/private/vsftpd.pem

COPY vsftpd.conf /etc/vsftpd.conf

EXPOSE 21 21100-21110

# Run vsftpd in foreground so Docker keeps the container alive
CMD ["/usr/sbin/vsftpd", "-obackground=NO", "/etc/vsftpd.conf"]
