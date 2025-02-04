# ProductAPI

An API that manages products and exposes RESTful endpoints. These endpoints should provide standard CRUD functionality for Products. We would also like to have the product Id to be auto-generated and this should be unique and have 6-digit number keeping in mind that the application can scale up, and it should not generate a duplicate id with different instances. 
Please use your imagination which fields can be present for product. Product Id and stock available must be present there, rest you can imagine yourself and add. 
Create an endpoint which gives ok response after internally decrementing the stock with the quantity provided in the url.  
Products 
 - POST 
/api/products 
 - GET 
/api/products 
/api/products/{id} 
 - DELETE 
/api/products/{id} 
 - PUT 
/api/products/{id} 
 - PUT 
/api/products/decrement-stock/{id}/{quantity} 
Create an endpoint which gives ok response after internally adding the stock with the quantity provided in the url. 
 - PUT 
/api/products/add-to-stock/{id}/{quantity} 
 
Every Product must have a unique number. When returning a list of products in any of the endpoint above, you must return the stock available for each product. 
Create a database using ef migrations project (code first approach). 

**How to Test API**

Please Add DB Connection value in appsetting.json file like - 
"ConnectionStrings": {
  "DefaultConnection": "DBConnectionValue"
}, 
Please run the ProductAPI project, it will open SwaggerUI page and it will provide all the api endpoints.
