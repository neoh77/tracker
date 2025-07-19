# Quick deployment script
#!/bin/bash
git clone https://github.com/yourusername/tracker.git
cd tracker
cp .env.example .env.production
# Edit .env.production with your values
docker-compose -f docker-compose.prod.yml up -d