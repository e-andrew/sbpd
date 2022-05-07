#Cerberus

## Authors:
Коренюк Андрій Олександрович (ФІОТ, ІВ-91)
Хандельди Олексій Романович (ФІОТ, ІВ-91)

## System requirements:
ОС - Windows
Мова виконання - С#
Запуск - через виконуваний файл

## Storage subsystem parameters:
Сховище (Storage) - це директорія ".storage", яка знаходиться в одній директорії із виконуваним файлом "Cerberus.exe". Якщо директорія ".storage" відсутня, то вона автоматично створюється.
Файл адміністратора (Config) - це файли із розширенням ".csv", що розміщені в директорії ".config". Якщо директорія ".config" відсутня, то вона автоматично створюється.
Блокування сховища відбувається за допомогою відкриття хендлера директорії ".storage".
Шляхи при передаванні іншим модулям перетворюються із абсолютних у відносні.
Шляхи при передаванні в модуль сховища перетворюються із відносних у абсолютні.
Елементами сховища (Item) є файли (мають тип "file") та директорії (мають тип "dir"). 
На даний момент користувач може виконувати наступні операції із елементами сховища:
* Open - applicated to - dir;
* Return - applied to - dir;
* Create - applied to - dir;
* Delete - applied to - file, dir;
* Read - applied to - file;
* Execute - applied to - file;
* Append - applied to - file;
* Rewrite - applied to - file;
* Rename - applied to - file, dir;
* Copy - applied to - file, dir;
* Replace - applied to - file, dir.

## Registration subsystem parameters:
Максимальне число користувачів (включаючи адміністратора) - 8.
Перший зареєстрований користувач стає адміністратором.
При реєстрації користувачу потрібно ввести логін, пароль і повторний пароль.
Вимоги до логіна:
1. Мінімальна довжина логіна - 1 символ.
2. Символи логіна повинні належати множині [.0-9@A-Za-z].
3. Логін унікальний.
Вимоги до пароля:
1. Мінімальна довжина пароля - 8 символів.
2. Максимальна довжина пароля - 20 символів.
3. Пароль та повторний пароль повинні бути ідентичними.
4. Символи пароля повинні належати множині [0-9@A-Za-z].
5. Пароль повинен містити мінімум одну цифру.
6. Пароль повинен містити мінімум один символ @.
7. Пароль повинен містити мінімум один символ високого регістру.
8. Пароль повинен містити мінімум один пароль низького регістру.
Пароль зберігається в хешованому вигляді.
Довжина солі для пароля - 20 символів.
Пароль дійсний 96 годин (пароль адміністратора - нескінченно довго).
Для користувача генерується код автентифікації. (користувачу його оголошує адміністратор)
Користувач не зможе ввійти в систему, поки не отримає підтвердження від адміністратора.
Видалити із системи можна будь-якого користувача, крім адміністратора.
Файл конфігурації - registration.csv
Формат записа файла конфігурації:
	login,salt,hashpassword,creationDate,expirationDate,acception,role,code

## Authentication subsystem parameters:
Автентифікація запускається паралельним потоком.
Секретна функція - floor(sqrt(x + a)). (користувачу її оголошує адміністратор)
Число х генерується в діапазоні [100, 1000) під час роботи модуля аутентифікації.
Число a генерується в діапазоні [100, 1000) під час реєстрації.
За сессію користувач повинен дати відповідь на 4 запитання.
Темп задавання питань - 1 хв.
Правила аутентифікації:
1. При проходженні аутентифікації користувач залишається в системі.
2. При помилці аутентифікації користувача вилогінює для повторного входу.
3. При 3 помилках аутентифікації користувача видаляє із системи. (крім адміністратора - його тільки вилогінює)
4. При відмові в проходженні аутентифікації користувача вилогінює із системи, але це не вважається помилкою аутентифікації.
Файл конфігурації - authentication.csv
Формат записа файла конфігурації:
	(REQUEST | SUCCESS | FAILURE),login,code,question,(requestAnswer | responseAnswer),(requestTime | responseTime)

## Authorisation subsystem parameters:
Користувач має 
Авторизація користувача відбувається в два етапи. На першому етапі для всіх елементів сховища визначається власник із файла конфігурації "owners.csv". На другому етапі для елемента сховища визначаються допустимі права із файлів конфігурації "rules_for_all.csv" і "rules_{login}.csv". Якщо у файлі конфігурації "owners.csv" відсутній запис про елемент сховища, то це значить, що його власником є будь-хто ("*"). Якщо у файлі конфігурації "rules..." відсутній запис про елемент сховища, то власник має всі права доступу. Записи в файлах конфігурації є обмежуючими.
Адміністратор має доступ до будь-якого файла і в нього є максимальні права доступу.
Права доступу визначаються бітами 32-бітного беззнакового цілого числа.
Dir:          Open | Create |    Х    | Delete | Copy | Rename | Replace
File: Read/Execute | Append | Rewrite | Delete | Copy | Rename | Replace
General:   Execute | Expand | Rewrite | Delete | Copy | Rename | Replace

Формат файлів конфігурації - owners.csv, rules_for_all.csv, rules_{login}.csv
Формат файла конфігурації "owners.csv":
	path,owner
Формат файла конфігурації "rules_for_all.csv":
	path,rights
Формат файла конфігурації "rules_{login}.csv":
	path,rights

## Monitoring subsystem parameters:
Запускається при вході користувача до системи.

| Danger Level | Action Type | Action Detail |
| :----------: | :---------: | :-----------: |
| 0-level | OPEN | [file or directory path] |
| 0-level | RETURN | [file or directory path] |
| 0-level | CREATE | [file or directory path] |
| 0-level | DELETE | [file or directory path] |
| 0-level | READ | [file or directory path] |
| 0-level | APPEND | [file or directory path] |
| 0-level | REWRITE | [file or directory path] |
| 0-level | RENAME | [path] : [old name] => [new name] |
| 0-level | COPY | [old path] => [new path] |
| 0-level | REPLACE | [old path] => [new path] |
| 0-level | LOGIN_PASS | without details |
| 0-level | AUTHENTICATION_PASS | without details |
| 1-level | LOGIN_ERROR | [login] |
| 2-level | AUTHORISATION_ERROR | without details |
| 3-level | AUTHENTICATION_ERROR | [request] => [response] |
| 4-level | FILESYSTEM_ERROR | [error message] |
| 4-level | DECIPHER_ERROR | [file or directory path] |
| 4-level | SYSTEM_TIME_CHANGE | without details |
| 4-level | ANOTHER_SYSTEM_EVENT | without details |

Формат файла конфігурації - log_{yyyy.MM.dd}.csv
Формат таблиці:
	ActionTime, login, DangerLevel, ActionType, ActionDetail.

## Cryptography Module
Зберігає функцію для генерації солі. Зберігає функцію для хешування.
Для цього модуля зарезервовано розширення ".cerenc".

## Testing
test.test@gmail.com
@tT2@tT2@tT2@tT2

test2
@tT2@tT2@tT2@tT2

test3
@tT2@tT2@tT2@tT2

test4
@tT2@tT2@tT2@tT2

test5
@tT2@tT2@tT2@tT2

test6
@tT2@tT2@tT2@tT2

test7
@tT2@tT2@tT2@tT2

test8
@tT2@tT2@tT2@tT2

## Troubleshooting
1. Необхідно додати перевірка на коректність введених даних.
2. Необхідно оптимізувати кількість звернень до модуля Storage для зменшення кількості операцій читання/запису в файл.
3. Необхідно додати адміністратору зручний функціонал для модифікації файлів конфігурації. Це допоможе уникнути мануальних помилок.

