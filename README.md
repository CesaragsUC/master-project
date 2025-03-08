# Casoft Store :rocket:

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=CesaragsUC_master-project&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=CesaragsUC_master-project)

**Casoft Store** is a Poc for a e-Commerce using .Net Core 8x.

Since it is a PoC, it does not have all the real features of an e-Commerce, so I added basic features of a virtual store.

## :slightly_smiling_face: Functionalities as User:

- Add items to cart
- Clear Cart
- Checkout
- Payment with credit card (simulation)
- Add product to favorites
- Apply discount coupons
- View my orders
- 
## :sunglasses: Functionalities as Administrator:

- View orders
- Create, Edit, Delete products
- Payments
- Pending and completed shipments
- View all users
- Sales metrics charts, etc.

#### Not all features were implemented 100% the same as in a real-world e-commerce because it is just a PoC. But it can serve as inspiration for future projects.

⭐ This can easily transformed into a Microservices, since each  service have your own context and there no accouplement.

Design Pattern:

- Clean Architecture & Distributed System

# :game_die: Technologies:

- CQRS
- Azure Blob Storage
- SonarQube
- Docker
- K8s
- Refit
- Keycloak
- RabbitMq
- PostgreSQL (write database)
- EntityFrameworkCore for PostgreSQL
- MongoDb (read only)
- Quartz for Background Services
- Serilog
- Ocelot for Gateway
- Jaeger OpenTelemetry
- Bogus for Unit Tests
- FluentValidation
- Redis
- Prometheus
- Grafana
- Azure keyVault
- Angular v18 for FrontEnd. You can find [HERE](https://github.com/CesaragsUC/master-project-web)

  ![design](https://github.com/user-attachments/assets/2199c7e4-16dc-4ce0-803c-86c16162abae)


⭐ Give a Start!

If you found this project useful, don't forget to give it a ⭐ on GitHub!
