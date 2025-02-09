To connect to the server...
1. Get your IP address from [here](https://checkip.amazonaws.com/).
2. Run `ssh-keygen -t rsa -b 2048 -f ~/.ssh/<your_key_name>`
3. On the server, open `~/.ssh/authorized_keys` and insert your public key.
4. On AWS, in the VPC settings under "security groups", add an inbound rule to your security group to allow in SSH traffic from your IP.
5. Now to connect to the server, run `ssh -i <path_to_key> ec2-user@18.117.172.148`.
