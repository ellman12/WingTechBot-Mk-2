# Run this to free up disk space on the EC2 instance.
# Docker can very quickly eat up disk space, so this is quite helpful.

sudo systemctl start docker

echo "Clearing old Docker containers and images"
docker system prune -a -f

echo "Clearing /tmp"
rm -rf /tmp/*

echo "Clearing NuGet cache"
rm -rf ~/.local/share/NuGet/http-cache/*


