-- Seed data for Organization
INSERT INTO "Organization" ("Id", "Name", "Inn", "Address")
VALUES ('a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11', 'Ромашка', '1234567890',
        '190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12'),
       ('a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a12', 'Рога и копыта', '0987654321',
        '230000, Иркутская обл., Октябрьский р-н, г. Иркутск, ул. 3 Июля, д. 3')
ON CONFLICT ("Id") DO NOTHING;

-- Get Organization IDs and seed related data
DO
$$
    DECLARE
        org1_id UUID := 'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11';
        cat1_id UUID;
        cat2_id UUID;
        emp1_id UUID;
        emp2_id UUID;
        nom1_id UUID;
        nom2_id UUID;
    BEGIN

        -- Seed data for Category
        INSERT INTO "Category" ("Id", "Name", "OwnerId")
        VALUES (gen_random_uuid(), 'Электроника', org1_id)
        RETURNING "Id" INTO cat1_id;

        INSERT INTO "Category" ("Id", "Name", "OwnerId")
        VALUES (gen_random_uuid(), 'Продукты', org1_id)
        RETURNING "Id" INTO cat2_id;

        -- Seed data for Employee
        INSERT INTO "Employee" ("Id", "Name", "PhoneNumber", "OwnerId", "Role")
        VALUES (gen_random_uuid(), 'Артём', '+79149021142', org1_id, 0)
        RETURNING "Id" INTO emp1_id;

        INSERT INTO "Employee" ("Id", "Name", "PhoneNumber", "OwnerId", "Role")
        VALUES (gen_random_uuid(), 'Иван', '+79902312001', org1_id, 1)
        RETURNING "Id" INTO emp2_id;

        -- Seed data for LoadingSettings
        INSERT INTO "LoadingSettings" ("Id", "OwnerId", "Description", "StartPosition", "BatchSize")
        VALUES (gen_random_uuid(), org1_id, 'Ежедневный базовый импорт', 1000, 500);

        -- Seed data for User
        INSERT INTO "User" ("Id", "Username", "PasswordHash", "Email", "OrganizationId", "CreatedAt", "UpdatedAt")
        VALUES (gen_random_uuid(), 'admin_org1', 'hashed_password_admin1', 'admin1@example.com', org1_id, NOW(), NOW()),
               (gen_random_uuid(), 'user_org1', 'hashed_password_user1', 'user1@example.com', org1_id, NOW(), NOW());

        -- Seed data for Nomenclature
        INSERT INTO "Nomenclature" ("Id", "Name", "CategoryId")
        VALUES (gen_random_uuid(), 'Ноутбук', cat1_id)
        RETURNING "Id" INTO nom1_id;

        INSERT INTO "Nomenclature" ("Id", "Name", "CategoryId")
        VALUES (gen_random_uuid(), 'Молоко', cat2_id)
        RETURNING "Id" INTO nom2_id;

        -- Seed data for Transaction
        INSERT INTO "Transaction" ("Id", "Type", "Amount", "Discount", "Quantity", "TransactionDate", "OwnerId",
                                   "NomenclatureId", "EmployeeId")
        VALUES (gen_random_uuid(), 1, 1200.00, 50.00, 1.00, '2023-01-15 10:30:00+00', org1_id, nom1_id, emp1_id),
               (gen_random_uuid(), 1, 3.50, 0.00, 2.00, '2023-01-15 11:00:00+00', org1_id, nom2_id, emp1_id),
               (gen_random_uuid(), 2, 10.00, 0.00, 1.00, '2023-01-16 09:15:00+00', org1_id, nom2_id, emp2_id);

    END
$$;