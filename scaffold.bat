dotnet ef dbcontext scaffold "server=localhost;port=3306;user=root;password=passw0rd;database=ToDo" Pomelo.EntityFrameworkCore.MySql -c ToDoDbContext -o Models -f --no-pluralize
