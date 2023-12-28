create database capstoneProject

use capstoneProject

create table credential
(
userid int identity primary key not null,
username varchar(100) not null,
password varchar(100) not null,
email varchar(100) not null,
roles varchar(100) not null
)
insert into credential values('superadmin','superadmin@123','superadmin@gmail.com','superadmin')
insert into credential values('kiran','kiran123','kiran@gmail.com','Admin')
insert into credential values('Balaji','balaji123','balaji@gmail.com','User')

--drop table credential


select * from credential

--creating table for image
--Atlast have to drop the table. while submiting the assignment.
--CREATE TABLE images (
--    P_id int identity primary key,
--    P_name nvarchar(50) NOT NULL,
--    P_image nvarchar(MAX) NOT NULL
--)

--select * from images

create table propertyDetails(
propertyID int not null primary key identity,
adminID int not null,
adminName varchar(100) not null,
propertyName varchar(100) not null,
Location varchar(100) not null,
price int not null,
category varchar(100) not null,
imageName varchar(1000) ,
Rating int,
)
select imageName from propertyDetails where propertyID=1

select * from propertyDetails

delete from propertyDetails where adminID=5

create table message(
id int not null primary key identity,
pid int not null,
sendermail varchar(100) not null,
message varchar(100),
date varchar(100),
)

select * from message

create table conversation(
id int not null primary key identity,
pid int not null,
senderMail varchar(100) not null,
customermail varchar(100) not null,
text varchar(1000) not null,
date datetime not null,
)

select * from conversation

create table propertyReservation(
reservationId int primary key identity not null,
propertyid int not null,
userid int not null,
adminid int not null,
propertyname varchar(100),
checkin datetime not null,
checkout datetime not null,
confirmation varchar(100)
)
select * from propertyReservation

drop table propertyReservation


