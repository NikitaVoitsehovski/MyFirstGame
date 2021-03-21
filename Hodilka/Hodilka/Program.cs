using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hodilka
{   /* Ходилка по пустому пространству, но с вероятностью нахождения монеток и магазина для покупок, а также врагов
     * Каждый шаг увеличивает счётчик шагов, и уменьшает здоровье на 1 ОЗ, найденные монетки собираются в кошель.
     * В магазине за  монетки можно купить предметы: плащ, яблоки или оружие
     * Больше информации в README файле!
     * 
     * Стрелки для перемещения
     * Пробел - взять/использовать
     * Esc - выйти из игры 
     * I - Информация о врагах
     * P - Съесть яблоко
      
  
     * Legend:
     * M - Money
     * T - Gansta
     * S - Shop
     * ░ - Block
     * E - Thief
     * F - Finish 
     * C - Chest
     * U - FortPost
     * GAME BY NIKITA VOITSEHOVSKI TA-20V
     */

    class Program
    {
        // Статические определения
        static int status = 20; // номер строки статуса
        static int msgrow = 21; // номер строки сообщения 
        static int warnrow = 22;// номер строки предупреждения
        // Вывод сообщения в строке msgrow
        
        static void Message(string msg)
        {
            Warning("");
            Console.SetCursorPosition(0, msgrow);
            for (int i = 0; i < 79; i++) Console.Write(' ');
            Console.SetCursorPosition(0, msgrow);
            Console.ForegroundColor = System.ConsoleColor.Green;
            Console.Write(msg);
        }
        // Вывод предупреждения в строке warnrow
        static void Warning(string msg)
        {
            Console.SetCursorPosition(0, warnrow);
            for (int i = 0; i < 79; i++) Console.Write(' ');
            Console.SetCursorPosition(0, warnrow);
            Console.ForegroundColor = System.ConsoleColor.Red;
            Console.Write(msg);
        }
        // Вывод статуса в строке status
        static void Status(int score, int eat, int dress, int weapon, int hp, int pistol, int dress_health)
        {   // стереть строку на экране в позиции вывода
            Console.SetCursorPosition(0, status); for (int i = 0; i < 80; i++) Console.Write(' ');
            Console.SetCursorPosition(0, status);
            Console.ForegroundColor = System.ConsoleColor.White;
            Console.Write($"Монетки:{score} шт. ");
            Console.ForegroundColor = System.ConsoleColor.Yellow;
            Console.Write($" Яблоки:{eat} шт. ");
            Console.ForegroundColor = System.ConsoleColor.Cyan;
            Console.Write($" Одежда:{dress} шт. ");
            Console.ForegroundColor = System.ConsoleColor.Green;
            Console.Write($" Нож:{weapon} шт. ");
            Console.ForegroundColor = System.ConsoleColor.DarkGreen;
            Console.Write($" Пистолет:{pistol} шт. ");
            Console.ForegroundColor = System.ConsoleColor.DarkRed;
            Console.Write($" Здоровье:{hp}"); 
        }
        // Вывод раскрашенного символа на цветном заднике, задник по умолчанию черный 
        static void ColorCharacter(char s, ConsoleColor c, ConsoleColor b = System.ConsoleColor.Black)
        {
            Console.ForegroundColor = c;
            Console.BackgroundColor = b;
            Console.Write(s);
            Console.BackgroundColor = System.ConsoleColor.Black;
        }
        static void Main(string[] args)
        {
            // Определим тип кодировки для ввода и вывода через консоль 
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            Random rnd = new Random(); // включаем генератор случайных чисел
            ConsoleKeyInfo action;     // обьект нажатой клавиши(действия)
            // Макет карты игрового поля
            // массив строк !! содержимое строк нельзя поменять
            string[] premap = {
                    "░░F ░  ░░░░░M ░  ░░░░░M ░  ░░░░░M ░  C ░  ░░░░░M ░  ░░░░░F░░",
                    "   M    S      C  ET       M    E  T E    U        ETE TETE ",
                    "░░E░ ░░  ░░  ░ ░░  ░░  ░ ░░  ░ ░   ░  ░S░░U ░░UU░ ░░UU░░UUU ",
                    "M E░  T     T░░     MT  ░       T░░    ░  U    T░░S    M CM░",
                    "░░T░░  ░C░░░ ░░  ░S░░░ ░░ M░S░░░ ░░MCE░░  ░S░░░ ░░M ░S░░░ ░S",
                    "M   ░  ░  E   ░  M  S      ░  E   ░    ░  ░  E   ░  M  m  T ",
                    "  ░░░  T░░  ░ ░   ░░  ░ ░ T ░░  ░ ░  ░ ░   ░░  ░ ░   ░░  ░ ░",
                    "M   ░░  ░░  ░ E  ░░░M       ░░░ ░ E    ░░  ░░░ ░ E  ░░░M  ░░",
                    "░   ░░    M░░     M ░   ░░     ░░    T ░░     ░░     E ░   T",
                    "M  ░  M░   T░░  ░░░  S      ░   T░░    ░   ░   T░░   ░  C  ░",
                    "░░E░░  ░M░░░ ░░ E ░░░░ ░░  ░S░░░ ░░ E ░░  ░S░░░ ░░ E ░░░░MM░",
                    "M     ░   M   ░  T  E   ░     E   ░ SS S ░   M   ░  T  E MM ",
                    " ░  ░S   ░░░░T   ░░░░░ M ░ ░░   ░  S S ░    ░░░░    ░░░░░░░░",
                    "M           ░░  M    S  ░       T░░ M  ░       T░░     MS  ░",
                    "    ░EE░T░░░░░░  ░E░░░░░░  ░S░░░ ░░   ░░T ░S░░░ ░░  ░S░░░ ░S",
                    "C  M░CM░         M  S         E              E   ░  M  S  T ",
            };
            int ymax = premap.Length;    // вычисляем размер макета по высоте
            int xmax = premap[0].Length; // и по ширине
            int y = ymax - 2; int x = 1; // определяем начальную позицию игрока
            // Создаём 2-х мерный массив символов нашей карты с возможностью удаления элементов на карте
            // с размерами макета
            char[,] map = new char[ymax, xmax];
            // Копируем из макета карты содержимое на карту посимвольно
            for (int i = 0; i < ymax; i++)
                for (int j = 0; j < xmax; j++)
                    map[i, j] = premap[i][j];

            int map_offset = 3; // смещение поля игры по вертикали
            int knife_price = 3; // цена в монетах за нож
            int dress_price = 8; // цена в монетах за плащ
            int pistol_price = 3;//цена пистолета
            int key_price = 7; //цена ключа
            int steps = 0;  // счётчик шагов
            int score = 0;  // кошелёк для монеток
            int dress = 0;  // количество купленной одежды
            int food = 1;   // количество купленной еды
            int knife = 0; // количество купленного оружия
            int weapon = knife; //было лень переписывать в каждом месте с Weapon на Knife
            int pistol = 0;
            int dress_health = 0; //здоровье куртки
            int key = 0; //кол-во ключей в начале игры
            int hp = 76;   //Здоровье героя
            bool noway = false; // флажок "ход невозможен"
            bool Key = false;
            Console.CursorVisible = false; // Убрать курсор

            Console.WriteLine("Бесконечная пошаговая бродилка с поиском монеток и покупкой вещей. Клавишы:\n" +
                    "↑ вперёд, ← влево, → вправо, ↓ назад, пробел взять/использовать, " +
                    "Esc выход\nНАЖМИ ЛЮБУЮ СТРЕЛОЧКУ ДЛЯ НАЧАЛА ИГРЫ");

            // считываем с клавиатуры символ очередного хода без отображения символа на экране
            // проверяем символ и итерируем до тех пор пока не нажата клавиша Escape
            Console.Beep(659, 125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(523, 125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(784, 125); Thread.Sleep(375); Console.Beep(392, 125); Thread.Sleep(375); Console.Beep(523, 125); Thread.Sleep(250); Console.Beep(392, 125); Thread.Sleep(250); Console.Beep(330, 125); Thread.Sleep(250); Console.Beep(440, 125); Thread.Sleep(125); Console.Beep(494, 125); Thread.Sleep(125); Console.Beep(466, 125); Thread.Sleep(42); Console.Beep(440, 125); Thread.Sleep(125); Console.Beep(392, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(784, 125); Thread.Sleep(125); Console.Beep(880, 125); Thread.Sleep(125); Console.Beep(698, 125); Console.Beep(784, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(587, 125); Console.Beep(494, 125); Thread.Sleep(125); Console.Beep(523, 125); Thread.Sleep(250); Console.Beep(392, 125); Thread.Sleep(250); Console.Beep(330, 125); Thread.Sleep(250); Console.Beep(440, 125); Thread.Sleep(125); Console.Beep(494, 125); Thread.Sleep(125); Console.Beep(466, 125); Thread.Sleep(42); Console.Beep(440, 125); Thread.Sleep(125); Console.Beep(392, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(784, 125); Thread.Sleep(125); Console.Beep(880, 125); Thread.Sleep(125); Console.Beep(698, 125); Console.Beep(784, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(587, 125); Console.Beep(494, 125); Thread.Sleep(375); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(415, 125); Console.Beep(440, 125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(440, 125); Console.Beep(523, 125); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(698, 125); Thread.Sleep(125); Console.Beep(698, 125); Console.Beep(698, 125); Thread.Sleep(625); Console.Beep(784, 125); Console.Beep(740, 125); Console.Beep(698, 125); Thread.Sleep(42); Console.Beep(622, 125); Thread.Sleep(125); Console.Beep(659, 125); Thread.Sleep(167); Console.Beep(415, 125); Console.Beep(440, 125); Console.Beep(523, 125); Thread.Sleep(125); Console.Beep(440, 125); Console.Beep(523, 125); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(622, 125); Thread.Sleep(250); Console.Beep(587, 125); Thread.Sleep(250); Console.Beep(523, 125); Thread.Sleep(1125);
            while ((action = Console.ReadKey(true)).Key != ConsoleKey.Escape)
            {
                noway = false;
                switch (action.Key)
                {
                    case ConsoleKey.UpArrow:
                        Message("Вперёд (-1 ОЗ)");
                        if (y > 0)
                        {
                            if (map[y - 1, x] != '░') { y = y - 1; steps++; hp--;}
                            else noway = true;
                        }
                        else noway = true;
                        break;
                    case ConsoleKey.DownArrow:
                        Message("Назад (-1 ОЗ)");
                        if (y + 1 < ymax)
                        {
                            if (map[y + 1, x] != '░') { y = y + 1; steps++; hp--;}
                            else noway = true;
                        }
                        else noway = true;
                        break;
                    case ConsoleKey.LeftArrow:
                        Message("Влево (-1 ОЗ)");
                        if (x > 0)
                        {
                            if (map[y, x - 1] != '░') { x = x - 1; steps++; hp--;}
                            else noway = true;
                        }
                        else noway = true;
                        break;
                    case ConsoleKey.RightArrow:
                        Message("Вправо (-1 ОЗ)");
                        if (x + 1 < xmax)
                        {
                            if (map[y, x + 1] != '░') { x = x + 1; steps++; hp--;}
                            else noway = true;
                        }
                        else noway = true;
                        break;
                    default:
                        noway = true;
                        break;
                }
                if (noway)
                {
                    Warning("Ударились об стену"); noway = false; hp--; 
                    Console.Beep();
                }
                for (int i = 0; i < ymax; i++)
                {
                    Console.SetCursorPosition(0, i + map_offset);
                    Console.ForegroundColor = System.ConsoleColor.Cyan;
                    for (int j = 0; j < xmax; j++)
                    {
                        switch (map[i, j])
                        {
                            case '░': ColorCharacter('█', System.ConsoleColor.Gray); break;
                            case 'T': ColorCharacter(map[i, j], System.ConsoleColor.Blue); break;
                            case 'S': ColorCharacter(map[i, j], System.ConsoleColor.DarkMagenta); break;
                            case 'E': ColorCharacter(map[i, j], System.ConsoleColor.Red); break;
                            case 'M': ColorCharacter(map[i, j], System.ConsoleColor.Yellow); break;
                            case 'F': ColorCharacter(map[i, j], System.ConsoleColor.Green); break;
                            case 'C': ColorCharacter(map[i, j], System.ConsoleColor.DarkGreen); break;
                            default: Console.Write(map[i, j]); break;
                        }
                    }
                }

                if (hp <= 0)
                {
                    Console.WriteLine("\nУ ТЕБЯ КОНЧИЛИСЬ ЖИЗНИ; ИГРА ОКОНЧЕНА"); break;
                }



                if (action.Key == ConsoleKey.P)
                {
                    
                    if (food == 1)
                        Message($"У вас есть {food} яблок\nВы сьели ЯБЛОКО"); hp += 20; food--;
         
                    if (food >= 0)
                        { 
                        Warning("У вас нет яблок!");
                        }
                }
   
                Status(score, food, dress, weapon, hp, pistol, dress_health);
                Console.SetCursorPosition(x, y + map_offset);
                ColorCharacter('☻', System.ConsoleColor.Cyan); // показываем лицо на чорном фоне

                if (map[y, x] == 'M') // если найдена монета, вы можете её забрать, нажав пробел
                {
                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.Cyan, System.ConsoleColor.Yellow); // лицо на жёлтом фоне(на монетке)

                    Message(" Ураа! Вы можете забрать монетку!!! нажмите пробел");
                    action = Console.ReadKey(true); // вводим клавишу
                    if (action.Key == ConsoleKey.Spacebar) // если нажат пробел
                    {
                        Console.SetCursorPosition(x, y + map_offset);
                        ColorCharacter('☻', System.ConsoleColor.Cyan); // монетку забрали, показываем лицо на чорном фоне
                        Console.Beep(100, 100); Console.Beep(400, 100); Console.Beep(100, 100);
                        Message("Взяли монетку (+1 монетка)");
                        map[y, x] = ' '; // удаляем эту монетку с карты
                        score++; // монеток на одну больше
                        steps++; // шагов тоже
                        action = Console.ReadKey(true);
                    }
                    else
                    {
                        Message("Не взяли монетку");
                        continue; // переходим к следующему циклу
                    }
                }
                else if (map[y, x] == 'T') // если гангстер, он забирает все монетки и убегает
                {
                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.Cyan, System.ConsoleColor.Blue); // лицо на синем фоне(на гангстере)

                    // звуковой SOS
                    Console.Beep(); Console.Beep(); Console.Beep();
                    System.Threading.Thread.Sleep(500);
                    Console.Beep(800, 500); Console.Beep(800, 500); Console.Beep(800, 500);
                    Console.Beep(); Console.Beep(); Console.Beep();

                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.Cyan); // Гангстер убежал, показываем лицо на чорном фоне
                    if (pistol <= 0)
                    {
                        Message(" Ой! Ганстер забрал ВСЕ твои монетки и убежал!!! А также он ударил вас (-25 ОЗ)");
                        map[y, x] = ' '; // удаляем гангстера с карты
                        score = 0; // монеток нет
                        steps++; // шагов на один больше
                        hp -= 25; //Теряем 25 очков здоровья
                        action = Console.ReadKey(true); // вводим клавишу
                    }

                    else

                    if (pistol >= 1)
                    {
                        Message("Вы прострелили колено ганстера! Он обронил свой кошель и яблоко!!!(Ваш пистолет сломан)");
                        map[y, x] = ' ';
                        score += 3;
                        steps++;
                        pistol--;
                        food++;
                        action = Console.ReadKey(true);
                    }

                }
                else if (map[y, x] == 'E') // враг, он вступает в бой с игроком
                {
                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.Cyan, System.ConsoleColor.Blue); // лицо на синем фоне(на враге)

                    // звуковой SOS
                    Console.Beep(); Console.Beep(); Console.Beep();
                    System.Threading.Thread.Sleep(500);
                    Console.Beep(800, 500); Console.Beep(800, 500); Console.Beep(800, 500);
                    Console.Beep(); Console.Beep(); Console.Beep();

                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.Cyan);

                    if (weapon >= 1)
                    {
                        Message("Вы вступили в бой и выиграли!!! Его монетки теперь ваши! (Ваш нож сломан!)");
                        map[y, x] = ' '; // удаляем врага с карты
                        score += 4; // отдал свои монетки
                        steps++; // шагов на один больше
                        weapon--;// теряем нож
                        action = Console.ReadKey(true); // вводим клавишу
                    }
                    else
                        if (weapon <= 0)
                    {
                        Message("У вас нет ножа с собой!В стычке с вором вы теряете все свои вещи(-25 ОЗ)");
                        map[y, x] = ' ';
                        score = 0;
                        weapon = 0;
                        weapon = 0;
                        pistol = 0;
                        hp -= 25;
                        action = Console.ReadKey(true);
                    }
                }

                else if (map[y, x] == 'C') // Сундук с замком
                {
                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.DarkBlue, System.ConsoleColor.DarkYellow);

                    Message("Вы нашли СУНДУК!!! Для открытия нужен ключ\nНажми 'E' для взаимодействия! ");
                    action = Console.ReadKey(true); // вводим клавишу
                    if (action.Key == ConsoleKey.E) // если нажатa E
                    {
                        Console.SetCursorPosition(x, y + map_offset);
                        ColorCharacter('☻', System.ConsoleColor.Yellow);
                        if (key >= 1)
                        {
                            Message("Вы открыли сундук (+12 монеток)");
                            map[y, x] = ' '; // удаляем сундук с карты
                            score += 12; // монеток на одну больше
                            steps++; // шагов тоже
                            key--;//теряем ключ
                            action = Console.ReadKey(true);
                        }
                        else
                        {
                            if (key <= 0)
                            {
                                Message("У вас нет КЛЮЧА!");
                            }
                            continue; // переходим к следующему циклу
                            if (steps % rnd.Next(1, 1) == 0) Key = true;
                            {
                                if (Key) Message("\nТы нашел ключ на полу\n"); key++;
                            }
                        }
                    }
                }
                else if (map[y, x] == 'U') //удаление куртки
                {
                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.White, System.ConsoleColor.White);
                    Message("Вас попросили сдать плащ для дальшейнейшего продвижения! За него вы получаете ЯБЛОКО, НОЖ И ПИСТОЛЕТ!!!");
                    dress = 0;
                    food++;
                    pistol++;
                    weapon--;
                }

                else if (map[y, x] == 'F') //Финиш игры
                {
                    Console.SetCursorPosition(x, y + map_offset);
                    ColorCharacter('☻', System.ConsoleColor.Green, System.ConsoleColor.Blue);
                    Console.Beep(784, 150);
                    Thread.Sleep(300);
                    Console.Beep(784, 150);
                    Thread.Sleep(300);
                    Console.Beep(932, 150);
                    Thread.Sleep(150);
                    Console.Beep(1047, 150);
                    Thread.Sleep(150);
                    Console.Beep(784, 150);
                    Thread.Sleep(300);
                    Console.Beep(784, 150);
                    Thread.Sleep(300);
                    Console.Beep(699, 150);
                    Thread.Sleep(150);
                    Console.Beep(740, 150);
                    Thread.Sleep(150);
                    Console.Beep(784, 150);
                    Thread.Sleep(300);
                    Console.Beep(784, 150);
                    Thread.Sleep(300);
                    Console.Beep(932, 150);
                    Thread.Sleep(150);
                    Console.Beep(1047, 150);
                    Thread.Sleep(150);
                    Console.Beep(784, 150);
                    Thread.Sleep(300);
                    Console.Beep(784, 150);
                    Thread.Sleep(300);
                    Console.Beep(699, 150);
                    Thread.Sleep(150);
                    Console.Beep(740, 150);
                    Thread.Sleep(150);
                    Console.Beep(932, 150);
                    Console.Beep(784, 150);
                    Console.Beep(587, 1200);
                    Thread.Sleep(75);
                    Console.Beep(932, 150);
                    Console.Beep(784, 150);
                    Console.Beep(554, 1200);
                    Thread.Sleep(75);
                    Console.Beep(932, 150);
                    Console.Beep(784, 150);
                    Console.Beep(523, 1200);
                    Thread.Sleep(150);
                    Console.Beep(466, 150);
                    Console.Beep(523, 150);
                    Console.WriteLine("УРА!!! ВЫ ПОБЕДИЛИ!!!\nЖМИ ВЫХОД (ESC) ДЛЯ ЗАВЕРШЕНИЯ ИГРЫ И СТАТИСТИКИ!!!");

                    //Особые концовки игры


                    if (steps >= 300)
                    {
                        Message($"ТЫ ПРОШЕЛ БОЛЬШЕ 300 ШАГОВ ({steps})\nТЫ НАСТОЯЩИЙ БЕГУЩИЙ В ЛАБИРИНТЕ!!1");
                    }
                    if (score >= 20)
                    {
                        Message($"ИЗ ЛАБИРИНТА ТЫ УНЕС {score} МОНЕТОК\nТЫ НАСТОЯЩАЯ ЛАРА КРОФТ!!1 ");
                    }
                    if (food >= 15)
                    {
                        Message($"ТЫ УНЕС С СОБОЙ {food} яблок\n ТЫ НАСТОЯЩИЙ ПЛЮШКИН!!1");
                    }

                }
                else if (dress >= 1)
                {

                    Message("Теперь вы не получаете урон от передвижений!!! "); hp++;
                }

                    
                

                if (action.KeyChar == 'i')
                
                    Warning($"В игре присутсвует 2 типа врагов:\n\nГангстер(T)-его можно победить только с помощью пистолета(из магазина),у вас их {pistol}\nВор(E)-его можно побежить только с помощью ножа,у вас их {weapon}\nПри проигрыше вору вы теряете ВСЕ свои вещи");
                    
                
                
    
                //МАГАЗИН


                else if (map[y, x] == 'S') // если найден магазин, то можно за монету купить вещь
                {
                    Message($"Вы нашли торговца; у вас имеется {score} монеток !!! ");
                    if (score > 0) // если кошелёк не пуст
                    {
                        Console.SetCursorPosition(x, y + map_offset);
                        ColorCharacter('☻', System.ConsoleColor.Black, System.ConsoleColor.Green);
                        Message($"Вы в магазине: 1-плащ, 2-еда, 3-нож, 4-пистолет, 5-ключ,6-цены");
                                  
                        action = Console.ReadKey(true); // считываем символ определяющий покупку без отображения символа на экране
                        switch (action.KeyChar)
                        {
                            case '1':
                                if (score >= dress_price)
                                {
                                    Message("Вы купили ПЛАЩ"); score -= dress_price; dress++;
                                }
                                else
                                if (dress >= 1)
                                {
                                    Message("У вас уже есть ПЛАЩ");
                                    continue;
                                }
                                else Warning("К сожалению, у вас не хватает монеток! Торговец ушел!");
                                break;

                            case '2':
                                Message("Вы купили ЯБЛОКО('P' для взаимодействия)"); score--; food++;
                                if (score <= 0)
                                {
                                    Warning("К сожалению, у вас не хватает монеток! Торговец ушел!");
                                }
                                break;

                            case '3':
                                if (score >= knife_price)
                                {
                                    Message("Вы купили НОЖ"); score -= knife_price; weapon++;
                                }
                                else Warning("К сожалению, у вас не хватает монеток! Торговец ушел");
                                break;

                            case '4':
                                if (score >= pistol_price)
                                {
                                    Message("Вы купили ПИСТОЛЕТ"); score -= pistol_price; pistol++;
                                }
                                else
                                    Warning("К сожалению, у вас не хватает монеток! Торговец ушел");
                                break;
                            case '5':
                                if (score >= key_price)
                                {
                                    Message("Вы купили КЛЮЧ"); key++;
                                }
                                else
                                    Warning("К сожалению, у вас не хватает монеток! Торговец ушел");
                                break;
                            case '6':
                                Message($"Плащ - 8 монеток Нож - 3 монетки Пистолет - 3 монетки");
                                continue;
                            default: Warning("Нет такого предмета!"); continue;
                        }
                    }  
                    else Warning("К сожалению, у вас нет монеток! Торговец ушел");
                    map[y, x] = ' '; //удаляем торговца для сложности игры
                }

            }


            //ФИНАЛЬНАЯ СТАТИСТИКА


            Message($"Ты прошёл {steps} шагов и заработал {score} монет\n" +
    $"у тебя было {food} яблок,{dress} плащ, с собой было {weapon} ножей, а пистолетов {pistol} штук\n");
            Console.WriteLine("Нажми любую клавишу для выхода");
            Console.ReadKey();
        }
    }
}