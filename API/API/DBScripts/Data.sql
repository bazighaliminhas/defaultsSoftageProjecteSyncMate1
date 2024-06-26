
SET IDENTITY_INSERT [dbo].[BookCategories] ON 

INSERT [dbo].[BookCategories] ([Id], [Category], [SubCategory]) VALUES (1, N'computer', N'algorithm')
INSERT [dbo].[BookCategories] ([Id], [Category], [SubCategory]) VALUES (2, N'mechanical', N'machine')
INSERT [dbo].[BookCategories] ([Id], [Category], [SubCategory]) VALUES (3, N'computer', N'programming languages')
INSERT [dbo].[BookCategories] ([Id], [Category], [SubCategory]) VALUES (4, N'computer', N'networking')
INSERT [dbo].[BookCategories] ([Id], [Category], [SubCategory]) VALUES (5, N'computer', N'hardware')
INSERT [dbo].[BookCategories] ([Id], [Category], [SubCategory]) VALUES (6, N'computer', N'operating systems')
INSERT [dbo].[BookCategories] ([Id], [Category], [SubCategory]) VALUES (7, N'mechanical', N'transfer of energy')
INSERT [dbo].[BookCategories] ([Id], [Category], [SubCategory]) VALUES (8, N'mathematics', N'calculus')
SET IDENTITY_INSERT [dbo].[BookCategories] OFF
GO
SET IDENTITY_INSERT [dbo].[Books] ON 

INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (2, N'Data Structures and Algorithms', N'Narsimha Karumanchi', 500, 1, 1)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (3, N'Let us C', N'Adam Drozdek', 400, 0, 3)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (5, N'Introduction to Algorithms', N'Thomas H. Cormen', 500, 1, 1)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (7, N'Introduction to Algorithms: A Creative Approach', N'Udi Manber', 500, 0, 1)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (8, N'Introduction to Algorithms: A Creative Approach', N'Udi Manber', 500, 0, 1)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (9, N'Introduction to Algorithms: A Creative Approach', N'Udi Manber', 500, 0, 1)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (12, N'Data Structures and Algorithms in C++', N'Adam Drozdek', 500, 0, 1)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (13, N'Data Structures and Algorithms in C++', N'Adam Drozdek', 500, 0, 1)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (14, N'Python for Everybody: Exploring Data Using Python 3', N'Charles Severance', 400, 0, 3)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (15, N'Java: A Beginner''s Guide', N'Herbert Schildt', 400, 0, 3)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (16, N'Java: A Beginner''s Guide', N'Herbert Schildt', 400, 0, 3)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (17, N'JavaScript: The Definitive Guide', N'David Flanagan', 900, 0, 3)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (18, N'The C++ Programming Language', N'Bjarne Stroustrup', 1000, 0, 3)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (19, N'The C++ Programming Language', N'Bjarne Stroustrup', 1000, 0, 3)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (20, N'The C++ Programming Language', N'Bjarne Stroustrup', 1000, 0, 3)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (21, N'The C++ Programming Language', N'Bjarne Stroustrup', 1000, 0, 3)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (22, N'The C++ Programming Language', N'Bjarne Stroustrup', 1000, 0, 3)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (23, N'Computer Networking: A Top Down', N'James Kurose and Keith Ross', 350, 0, 4)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (24, N'Computer Networking: A Top Down', N'James Kurose and Keith Ross', 350, 0, 4)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (25, N'Data Communications and Networking', N'Behrouz A. Forouzan', 670, 0, 4)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (26, N'Introduction to Networking: How the Internet Works', N'Charles Severance', 600, 0, 4)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (27, N'Introduction to Networking: How the Internet Works', N'Charles Severance', 600, 0, 4)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (28, N'Computer Networking for Beginners', N'Russell Scott', 600, 0, 4)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (29, N'Microprocessor 80386 Hardware Reference Manual', N'Intel', 2000, 0, 5)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (30, N'Microprocessor 80386 Hardware Reference Manual', N'Intel', 2000, 0, 5)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (31, N'Microprocessor 80387 Hardware Reference Manual', N'Intel', 2000, 0, 5)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (32, N'Microprocessor 80387 Hardware Reference Manual', N'Intel', 2000, 0, 5)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (33, N'Microprocessor 8085', N'Ramesh Gaonkar', 2000, 0, 5)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (34, N'Microprocessor 8085', N'Ramesh Gaonkar', 2000, 0, 5)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (35, N'Microprocessor 8086', N'Ramesh Gaonkar', 2000, 0, 5)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (36, N'Microprocessor 8086', N'Ramesh Gaonkar', 2000, 0, 5)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (37, N'Operating System Concepts', N'Abraham Silberschatz and Peter Galvin', 1500, 0, 6)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (38, N'Operating System Concepts', N'Abraham Silberschatz and Peter Galvin', 1500, 0, 6)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (39, N'Design of the UNIX Operating Systems', N'Maurice Bach', 1500, 0, 6)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (40, N'Design of the UNIX Operating Systems', N'Maurice Bach', 1500, 0, 6)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (41, N'Operating System: A Design-oriented Approach', N'Charles Crowley', 1500, 0, 6)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (42, N'Operating Systems: A Concept-Based Approach', N'D M Dhamdhere', 1500, 0, 6)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (43, N'Operating Systems: A Concept-Based Approach', N'D M Dhamdhere', 1500, 0, 6)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (44, N'Fluid Mechanics and Hydraulic Machines', N'Dr. R K Bansal', 1000, 0, 7)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (45, N'Fluid Mechanics and Hydraulic Machines', N'Dr. R K Bansal', 1000, 0, 7)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (50, N'An Introduction to Mechanics', N'David Kleppne', 1000, 0, 2)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (51, N'An Introduction to Mechanics', N'David Kleppne', 1000, 0, 2)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (52, N'Theory of Machines', N'SS Rattan', 1000, 0, 2)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (53, N'Theory of Machines', N'SS Rattan', 1000, 0, 2)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (54, N'Design of Machine Elements', N'V B Bhandari', 1200, 0, 2)
INSERT [dbo].[Books] ([Id], [Title], [Author], [Price], [Ordered], [CategoryId]) VALUES (55, N'Fundamentals of Thermodynamics', N'Claus Borgnakke', 1200, 0, 7)
SET IDENTITY_INSERT [dbo].[Books] OFF
GO


INSERT INTO [dbo].[Users]
           ([FirstName]
           ,[LastName]
           ,[Email]
           ,[Mobile]
           ,[Password]
           ,[Blocked]
           ,[Active]
           ,[CreatedOn]
           ,[UserType])
     VALUES
           ('Admin'
           ,'Admin'
           ,'admin@gmail.com'
           ,'03215678861'
           ,'admin1122'
           ,0
           ,1
           ,GETDATE()
           ,'ADMIN')
GO


