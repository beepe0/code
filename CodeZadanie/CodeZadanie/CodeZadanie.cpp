#include <iostream>
#include <limits>
using namespace std;





int main()
{
    int arbuz;
    int temp;
    int lp = 0;
    int countl;
    int nextl = 2;
    cout << "Podaj ile chcesz wygenerować liczb: ";
    cin >> arbuz;
    
    while(lp < arbuz)
    {
        for(int d = 2; d < nextl-1; d++)
        {
            if(nextl % d == 1)
            {
                countl++;
                //cout << " " << nextl;
            }
            
        }
        if(countl > 2 || countl == 0)cout << " " << nextl;
        nextl++;
        lp++;
    }
    
}