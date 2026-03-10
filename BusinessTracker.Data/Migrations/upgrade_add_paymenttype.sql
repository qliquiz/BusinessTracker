-- Add PaymentType column to Transactions (1=Cash, 2=NonCash, 3=Other)
ALTER TABLE "Transactions"
    ADD COLUMN IF NOT EXISTS "PaymentType" INT NOT NULL DEFAULT 1;
