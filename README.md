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
	В командной строке заполните ключи api и header:
	```
	dotnet user-secrets set api <YOUR KEY>
	Пример:
	dotnet user-secrets set api 12354

	dotnet user-secrets set header <AUTHORIZATION HEADER>
	(добавил на всякий случай)
	Пример:
	dotnet user-secrets set header no 
	```
3. После успешного задания значений запустите проект и дождитесь загрузки пакетов
4. Запустите проект
	