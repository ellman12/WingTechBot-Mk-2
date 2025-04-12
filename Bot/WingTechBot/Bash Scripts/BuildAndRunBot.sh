sudo systemctl start docker

docker build --no-cache -t wingtech-bot . && docker-compose up -d --force-recreate

