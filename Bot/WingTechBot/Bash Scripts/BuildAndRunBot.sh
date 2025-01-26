# Automatically starts Docker, pulls master (or the branch you give it), and builds/starts the bot.

if [ -z "$1" ]; then
	branch="master"
else
	branch=$1
fi

clear

cd ~

sudo systemctl start docker

if [ -d "./WingTechBot" ]; then
	cd WingTechBot
	git switch $branch
	git fetch
	git reset --hard origin/$branch
else
	git clone https://github.com/ellman12/WingTechBot
	cd WingTechBot
fi

cd Bot/WingTechBot

# -d can be used to hide compose's output.
docker build --no-cache -t wingtech-bot . && docker-compose up --force-recreate
