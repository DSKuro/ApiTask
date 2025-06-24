# ApiTask
## Инструкция по использованию
1. Клонируйте/скачайте проект
2. Задайте значения в user-secrets:\
В командной строке дойдите до корневой папки проекта.\
	```
	...\ApiTask

	Пример:
	D:\Repos\ApiTask
	```
	В командной строке заполните ключи api и header, строку подключения к базе данных:
	```
	dotnet user-secrets set api <YOUR KEY>
	Пример:
	dotnet user-secrets set api 12354

	dotnet user-secrets set header <AUTHORIZATION HEADER>
	(добавил на всякий случай)
	Пример:
	dotnet user-secrets set header no 
	
	dotnet user-secrets set db <CONNECTION STRING>
	CONNECTION STRING:
	По логину и паролю
	Server=адрес_сервера;Database=имя_базы_данных;User Id=логин;Password=пароль;
	По аутентификации Windows:
	Server=адрес_сервера;Database=имя_базы_данных;Trusted_Connection=True;
	Пример:
	Server=.\SQLEXPRESS;Database=DATA.MDF;Trusted_Connection=True;TrustServerCertificate=True
	```
3. После успешного задания значений запустите проект и дождитесь загрузки пакетов
4. Запустите проект
	
