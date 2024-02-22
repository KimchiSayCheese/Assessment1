CREATE DATABASE Part;

DROP TABLE IF EXISTS item;

CREATE TABLE item (
    id SERIAL PRIMARY KEY,
    item_name VARCHAR(50) NOT NULL,
    parent_item INTEGER,
    cost INTEGER NOT NULL,
    req_date DATE NOT NULL,
    CONSTRAINT fk_parent_item FOREIGN KEY (parent_item) REFERENCES item (id)
);
INSERT INTO item(item_name, parent_item, cost, req_date) VALUES
    ('Item1', NULL, 500, '2024-02-20'),
    ('Sub1', 1, 200, '2024-02-10'),
    ('Sub2', 1, 300, '2024-01-05'),
    ('Sub3', 2, 300, '2024-01-02'),
    ('Sub4', 2, 400, '2024-01-02'),
    ('Item2', NULL, 600, '2024-03-15'),
    ('Sub1', 6, 200, '2024-02-25');

CREATE OR REPLACE FUNCTION Get_Total_Cost(itemName VARCHAR)
    RETURNS INTEGER AS $$
DECLARE
    totalCost INTEGER := 0;
    parentIsNull BOOLEAN := FALSE;
BEGIN
    SELECT TRUE INTO parentIsNull
    FROM item
    WHERE item_name = itemName AND parent_item IS NULL;
    
    IF NOT parentIsNull THEN
        RETURN NULL;
    END IF;
    
    WITH RECURSIVE item_total AS (
        SELECT id, cost
        FROM item
        WHERE item_name = itemName
        UNION ALL
        SELECT i.id, i.cost
        FROM item AS i 
        JOIN item_total ih ON i.parent_item = ih.id
    )
    SELECT SUM(cost)
    INTO totalCost
    FROM item_total;
    RETURN totalCost;
END;
$$ LANGUAGE plpgsql;
