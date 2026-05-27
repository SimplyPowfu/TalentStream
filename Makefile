DOCKER_COMPOSE = docker compose
INFRA_FILE     = docker-compose.yml
API_PROJECT    = src/TalentStream.WebApi
INFRA_PROJECT  = src/TalentStream.Infrastructure

CYAN  := \033[0;36m
GREEN := \033[0;32m
NC    := \033[0m

all: up

up:
	$(DOCKER_COMPOSE) -f $(INFRA_FILE) up -d --build
	@echo ""
	@echo "$(GREEN)==================================================$(NC)"
	@echo "$(GREEN)   Servizi avviati con successo! $(NC)"
	@echo "$(GREEN)==================================================$(NC)"
	@echo "Puoi accedere alle interfacce web ai seguenti link:"
	@echo ""
	@echo "  $(CYAN)Swagger:$(NC)       http://localhost:5000/swagger/index.html"
	@echo "  $(CYAN)Adminer:$(NC)       http://localhost:8081/"
	@echo "  $(CYAN)MongoExpress:$(NC)  http://localhost:8082/"
	@echo ""
	@echo "$(CYAN)Ricorda di lanciare Make update per creare i db$(NC)"
	@echo "$(GREEN)==================================================$(NC)"

down:
	$(DOCKER_COMPOSE) -f $(INFRA_FILE) down

backend:
	docker compose up -d --build backend

restart:
	$(DOCKER_COMPOSE) -f $(INFRA_FILE) down --remove-orphans
	$(DOCKER_COMPOSE) -f $(INFRA_FILE) up -d
	
build:
	dotnet build

update: build
	dotnet ef database update --project $(INFRA_PROJECT) --startup-project $(API_PROJECT)

migration:
	@read -p "Inserisci il nome della migrazione (es. AddPhoneToUser): " msg; \
	dotnet ef migrations add $$msg --project $(INFRA_PROJECT) --startup-project $(API_PROJECT); \
	dotnet ef database update --project $(INFRA_PROJECT) --startup-project $(API_PROJECT); \

clean:
	$(DOCKER_COMPOSE) -f $(INFRA_FILE) down --remove-orphans

fclean:
	$(DOCKER_COMPOSE) -f $(INFRA_FILE) down --remove-orphans -v

re: fclean up update


.PHONY: all up down backend restart build update clean fclean re