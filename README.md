# ApiTask
## ���������� �� �������������
1. ����������/�������� ������
2. ������� �������� � user-secrets:\
� ��������� ������ ������� �� �������� ����� �������.\
	```
	...\ApiTask

	������:
	D:\Repos\ApiTask
	```
	� ��������� ������ ��������� ����� api � header, ������ ����������� � ���� ������:
	```
	dotnet user-secrets set api <YOUR KEY>
	������:
	dotnet user-secrets set api 12354

	dotnet user-secrets set header <AUTHORIZATION HEADER>
	(������� �� ������ ������)
	������:
	dotnet user-secrets set header no 
	
	dotnet user-secrets set db <CONNECTION STRING>
	CONNECTION STRING:
	�� ������ � ������
	Server=�����_�������;Database=���_����_������;User Id=�����;Password=������;
	�� �������������� Windows:
	Server=�����_�������;Database=���_����_������;Trusted_Connection=True;
	������:
	Server=.\SQLEXPRESS;Database=DATA.MDF;Trusted_Connection=True;TrustServerCertificate=True
	```
3. ����� ��������� ������� �������� ��������� ������ � ��������� �������� �������
4. ��������� ������
	