FROM alpine:latest

RUN apk add --no-cache postgresql-client bash curl tzdata

ENV TZ=UTC

COPY ./Backup.sh /Backup.sh
RUN chmod +x /Backup.sh

RUN echo "0 0 * * * /bin/sh /Backup.sh >> /var/log/cron.log 2>&1" | crontab -

CMD ["crond", "-f"]

