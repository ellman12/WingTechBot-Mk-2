CREATE TABLE karma (
	id SERIAL PRIMARY KEY,
	giver text NOT NULL,
	receiver text NOT NULL,
	amount INTEGER CHECK (amount > 0)
);

