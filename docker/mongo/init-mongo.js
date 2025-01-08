
db.createUser({
    user: "cesar",
    pwd: "cesar",
    roles: [
      { role: "readWrite", db: "CasoftStore" },
      { role: "readWrite", db: "SimpleDbDemo" },
      { role: "readWrite", db: "admin" }
    ]
  });

  db = new Mongo().getDB("SimpleDbDemo");

  db.createCollection("SimpleDbDemo");

  db.SimpleDbDemo.insertOne({
    "ProductId": "e245917f-5f49-492b-bff7-74e304cfea82", 
    "Name": "Battlefield VI", 
    "Price": NumberDecimal("150"), 
    "Active": true,
    "ImageUri": "url da imagem", 
    "CreatAt": ISODate("2024-12-24T17:00:00.000+0000")
});
