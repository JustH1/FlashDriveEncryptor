FDE is a console program for encrypting and decrypting files on USB drives or other directories specified in the configuration.

Available сommands:
| Команда          | Аргумент       | Действие                              | Пример                     |
|------------------|----------------|---------------------------------------|----------------------------|
| `help`           | —              | Показать справку                      | `help`                     |
| `enc`            | `<password>`   | Зашифровать файлы                     | `enc MyPassword123`        |
| `dec`            | `<password>`   | Расшифровать файлы                    | `dec MyPassword123`        |
| `exit`           | —              | Выйти из программы                    | `exit`                     |

Before using it, you must configure the parameters in the configuration file (for example, specify the path to the encrypted directory). The configuration file (AppConfiguration.json) must be located in the application directory.
