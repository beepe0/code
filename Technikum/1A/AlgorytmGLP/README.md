# Algorytm wyznaczania liczb pierwszych

# Wynik
![Algorytm](https://cdn.discordapp.com/attachments/947215628983500850/955914843091771422/obraz_2022-03-22_203937.png)
# Przykład
![Algorytm](https://cdn.discordapp.com/attachments/947215628983500850/955915199196565595/unknown.png)
# Przykład
Na początku ja stworzyłem projekt i zacząłem pisać, stworzyłem 4 zmieny, pierwsza nazywa się „countTime”, ta zmiana znaczy, ile liczb pokazać, druga zmiana „lp”, ta zmiena znaczy, obecny rachunek, trzecia „countl”, ta zmiana znaczy, ile są dzielników dla liczby, a ostatnia „nextl”, to jest liczba pierwsza. Potem ja napisałem w konsole „Podaj ile chcesz wygenerować liczb:”, potem wpisz ile razy wygenerować liczb. Idziemy dalej, stworzył pętlą "while", i sprawdzam czy zmiana „lp” mniej „countTime”, jeżeli tak to idzie dalie lub koniec, potem używam pętlą "for" і pisze coś takiego, jak „for(int d = 2; d <= nextl - 1; d++)”, zmiena „d” będzie przechowyć dzielnik do liczby „nextl”, potem w tej pętle sprawdzam czy liczba „d” może być podzielona przez „nextl”, coś takiego „if(nextl % d == 0)”, jeżeli to jest prawda to w zmieną „countl” dodaję 1 to znaczy że jest nowy dzielnik, potem sprawdzam jeżeli dzielników równo 0 to pisze liczbę w konsole, i robię następny krok „lp++”, inaczej następna liczba „nextl++”, i zeruję „countl”, to dopóki „lp” będzie równe „countTime”. 

# Wniosek 
Ta programma ma 10 kroków:
+ 1 krok - piszemy ilość wygenerowanych liczb;
+ 2 krok - pracujemy dopoki lp < countTime;
+ 3 krok - liczby przez który my możemy podzielić nextl;
+ 4 krok - sprawdziamy czy mozemy my podzielić, jeżeli tak to idziemy do kroku 5;
+ 5 krok - dodać jeden dzielnik do countl;
+ 6 krok - sprawdziamy czy countl > 0, jeżeli tak to idziemy do 7 kroku i 8 kroku;
+ 7 krok - piszemy liczbę w konsole;
+ 8 krok - dodajemy plus jeden do ilość wygenerowanych liczb;
+ 9 krok - następna nowa liczba;
+ 10 krok - zerujemy countl;

Programa skonczyła z 15 wugenerowanych liczb swoje prace za 3 millisekundy, ale to zależy od ilości wygenerowanych liczb;
