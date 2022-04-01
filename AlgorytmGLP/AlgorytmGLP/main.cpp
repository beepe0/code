#include <iostream>
//#include <chrono>
//#include <unistd.h>
using namespace std;

int main()
{
    int countTime;
    int lp = 0;
    int countl = 0;
    int nextl = 2;
    cout << "Podaj ile chcesz wygenerować liczb: ";
    cin >> countTime; //1 krok
    //auto start = chrono::steady_clock::now();
    while(lp < countTime) //2 krok
    {
        for(int d = 2; d <= nextl - 1; d++) // 3 krok
                    // 4 krok  // 5 krok
            if(nextl % d == 0){countl++;}
            // 6 krok        // 7 krok       // 8 krok
        if(countl == 0){ cout << " " << nextl; lp++;}
        nextl++; // 9 krok
        countl = 0; // 10 krok
    }
    //auto end = chrono::steady_clock::now();
    //cout << chrono::duration_cast<chrono::milliseconds>(end - start).count() << " mill";
}