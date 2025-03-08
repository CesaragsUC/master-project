db = db.getSiblingDB("admin");

if (!db.getUser("cesar")) {
    db.createUser({
        user: "cesar",
        pwd: "cesar",
        roles: [
            { role: "root", db: "admin" },
            { role: "readWrite", db: "CasoftStore" }
        ]
    });
} else {
    db.updateUser("cesar", {
        roles: [
            { role: "root", db: "admin" },
            { role: "readWrite", db: "CasoftStore" }
        ]
    });
}
