DOCKER_COMPOSE = docker compose
INFRA_FILE     = docker-compose.yml
API_PROJECT    = src/TalentStream.WebApi
INFRA_PROJECT  = src/TalentStream.Infrastructure

GREEN          = \033[0;32m
RED            = \033[0;31m
RESET          = \033[0m

all: up

up:
	@echo "$(GREEN)Starting Docker Infrastructure...$(RESET)"
	$(DOCKER_COMPOSE) -f $(INFRA_FILE) up -d
	@echo "$(GREEN)Infrastructure is up! Adminer: http://localhost:8081/ | MongoExpress: http://localhost:8082/$(RESET)"

down:
	@echo "$(RED)Stopping Docker Infrastructure...$(RESET)"
	$(DOCKER_COMPOSE) -f $(INFRA_FILE) down
	@echo "$(RED)Infrastructure stopped.$(RESET)"

restart:
	@echo "$(GREEN)Restarting Infrastructure...$(RESET)"
	$(DOCKER_COMPOSE) -f $(INFRA_FILE) down --remove-orphans
	$(DOCKER_COMPOSE) -f $(INFRA_FILE) up -d
	@echo "$(GREEN)Infrastructure restarted successfully.$(RESET)"

build:
	@echo "$(GREEN)Compiling .NET Solution...$(RESET)"
	dotnet build

update: build
	@echo "$(GREEN)Applying EF Core Migrations to SQL Server...$(RESET)"
	dotnet ef database update --project $(INFRA_PROJECT) --startup-project $(API_PROJECT)
	@echo "$(GREEN)Database structure aligned!$(RESET)"

clean:
	@echo "$(RED)Cleaning up containers and orphan resources...$(RESET)"
	$(DOCKER_COMPOSE) -f $(INFRA_FILE) down --remove-orphans

fclean:
	@echo "$(RED)WARNING: Purging ALL containers, networks and database VOLUMES...$(RESET)"
	$(DOCKER_COMPOSE) -f $(INFRA_FILE) down --remove-orphans -v
	@echo "$(RED)Full wipe completed. SQL Server data has been reset.$(RESET)"

reinit: fclean up ef-update

status:
	@echo "$(GREEN)Current Container Status:$(RESET)"
	$(DOCKER_COMPOSE) -f $(INFRA_FILE) ps -a

.PHONY: all up down restart reinit clean fclean status build ef-update