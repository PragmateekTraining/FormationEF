IF DB_ID('movies') IS NOT NULL
BEGIN
	ALTER DATABASE movies SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE movies
END

CREATE DATABASE movies

USE movies

CREATE TABLE movies
(
	id INT PRIMARY KEY IDENTITY,
	title TEXT
)

CREATE TABLE actors
(
	id INT PRIMARY KEY IDENTITY,
	name TEXT
)

CREATE TABLE performances
(
	movie_id INT NOT NULL FOREIGN KEY REFERENCES movies(id),
	actor_id INT NOT NULL FOREIGN KEY REFERENCES actors(id),
	fee INT NOT NULL
)

INSERT INTO movies(title) VALUES ('Mission: Impossible')
INSERT INTO movies(title) VALUES ('Matrix')
INSERT INTO movies(title) VALUES ('War of the Worlds')

INSERT INTO actors(name) VALUES ('Tom Cruise')
INSERT INTO actors(name) VALUES ('Keanu Reeves')

INSERT INTO performances(movie_id, actor_id, fee) VALUES (1, 1, 290000000)
INSERT INTO performances(movie_id, actor_id, fee) VALUES (2, 2, 262000000)
INSERT INTO performances(movie_id, actor_id, fee) VALUES (3, 1, 100000000)