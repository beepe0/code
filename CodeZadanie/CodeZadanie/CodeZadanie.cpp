#include <iostream>
using namespace std;

int main()
{
    int countTime;
    int lp = 0;
    int countl = 0;
    int nextl = 2;
    cout << "Podaj ile chcesz wygenerować liczb: ";
    cin >> countTime;

    while(lp < countTime)
    {
        for(int d = 2; d <= nextl - 1; d++)
            if(nextl % d == 0){countl++;}
        if(countl == 0){ cout << " " << nextl; lp++;}
        nextl++;
        countl = 0;
    }
}