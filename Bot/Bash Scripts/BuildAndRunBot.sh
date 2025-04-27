sudo systemctl start docker

cd Frontend
docker build --no-cache -t wingtech-bot-frontend .

cd ../WingTechBot
docker build --no-cache -t wingtech-bot .

cd ..
docker-compose up --force-recreate

