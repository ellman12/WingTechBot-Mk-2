#!/bin/sh

TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_FILE="/backups/${DATABASE_NAME}_$TIMESTAMP.sql.gz"

echo "Starting backup: $BACKUP_FILE"

pg_dump -h $DATABASE_HOST -U $DATABASE_USER $DATABASE_NAME | gzip > $BACKUP_FILE

echo "Backup complete"

