-- Seed data for Organizations
INSERT INTO "Organizations" ("Id", "Name", "Inn", "Address")
VALUES ('a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11', 'Главный офис (Спб)', '1234567890',
        '190000, Ленинградская обл., Ломоносовский р-н, г. Ломоносов, ул. Советская, д. 12'),
       ('a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a12', 'Филиал (Иркутск)', '0987654321',
        '230000, Иркутская обл., Октябрьский р-н, г. Иркутск, ул. 3 Июля, д. 3') ON CONFLICT ("Id") DO NOTHING;

-- Seed data for Users
INSERT INTO "Users" ("Name", "Password")
VALUES ('Администратор', 'admin123') ON CONFLICT ("Id") DO NOTHING;

-- Seed data for Categories
INSERT INTO "Categories" ("Id", "Name", "OwnerId")
VALUES ('c1eebc99-9c0b-4ef8-bb6d-6bb9bd380a11', 'Продукты питания',
        'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11') ON CONFLICT ("Id") DO NOTHING;

-- Seed data for Nomenclatures
INSERT INTO "Nomenclatures" ("Id", "Name", "CategoryId")
VALUES ('d1eebc99-9c0b-4ef8-bb6d-6bb9bd380a11', 'Хлеб пшеничный',
        'c1eebc99-9c0b-4ef8-bb6d-6bb9bd380a11') ON CONFLICT ("Id") DO NOTHING;

-- Seed data for Employees
INSERT INTO "Employees" ("Id", "Name", "PhoneNumber", "OwnerId", "Role")
VALUES ('e1eebc99-9c0b-4ef8-bb6d-6bb9bd380a11', 'Иван Иванов', '+79991234567', 'a0eebc99-9c0b-4ef8-bb6d-6bb9bd380a11',
        1) ON CONFLICT ("Id") DO NOTHING;
