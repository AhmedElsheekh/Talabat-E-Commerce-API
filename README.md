# Talabat-E-Commerce-API
 "Talabat" is a robust and scalable ASP.NET-based API designed specifically for e-commerce applications. It provides a comprehensive set of endpoints for managing products, orders, customers, and other essential e-commerce functionalities. With it, developers can easily integrate and extend e-commerce capabilities into web and mobile applications

## Technologies Used
- ASP.NET Core
- Entity Framework Core
- Swagger/OpenAPI (optional, for API documentation)
- SQL Server

## Prerequisites
1. Visual Studio (or Visual Studio Code)
2. .NET SDK
3. SQL Server
4. Postman (optional, for testing API endpoints)

## Installation
1. Clone the repository: `git clone https://github.com/your-repository.git`
2. Open the solution in Visual Studio.
3. Restore NuGet packages: `dotnet restore`
4. Update the database connection string in `appsettings.json` or `appsettings.Development.json`.
5. Apply database migrations: `dotnet ef database update`
6. Build and run the project.

## Configuration
- Modify `appsettings.json` to configure database connection strings, API keys (if any), and other settings.
- Update CORS policies in `Program.Main` to allow specific origins for API requests.

## Endpoints
1. **Accounts**
   - POST `/api/Accounts/Register`: Register a new customer.
   - POST `/api/Accounts/Login`: Log a customer.
   - GET `/api/Accounts/GetCurrentUser`: Get the current signed in user.
   - GET `/api/Accounts/Address`: Get the address of the current signed in user.
   - PUT `/api/Accounts/Address`: Update the address for the current signed in user.
   - GET `/api/Accounts/EmailExists`: Check if this email is registered before.
  
 2. **Baskets**
   - GET `/api/Baskets`: Get the basket by id if exists, otherwise create a new empty basket with the same id.
   - POST `/api/Baskets`: Create a new basket with the given items.
   - Delete `/api/Baskets`: Delete a basket.

3. **Orders**
   - POST `/api/orders`: Create a new order.
   - GET `/api/orders/`: Get all orders for a specific user.
   - GET `/api/orders/{OrderId}`: Retrieve a specific order.
   - GET `/api/orders/DeliveryMethod`: Get all delivery methods.
     
4. **Payments**
   - POST `/api/Payments`: To start a payment process.
   - POSt `/api/Payments/webhook`: Enable the payment gateway to send our app the payment status.
     
 5. **Products**
   - GET `/api/Products`: Retrieve all products.
   - GET `/api/Products/{id}`: Retrieve a specific product by ID.
   - GET `/api/Products/Types`: Retrieve all product types.
   - GET `/api/Products/Brands`: Retrieve all product brands.

## Usage
1. Use Postman or any API testing tool to interact with the endpoints.
2. Authenticate using `/api/Accounts/Login` to obtain an access token (Bearer token).
3. Include the access token in the Authorization header for protected endpoints (Bearer {token}).
4. Send requests to the desired endpoints based on the API documentation.

## Contributing
1. Fork the repository.
2. Create a new branch (`git checkout -b feature/my-feature`).
3. Commit your changes (`git commit -am 'Add new feature'`).
4. Push to the branch (`git push origin feature/my-feature`).
5. Create a new Pull Request.



## Contact
For any questions or support, please contact [ahmed.elsheekh8620@gmail.com](mailto:your-email@example.com).

