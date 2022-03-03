#include <iostream>
using namespace std;
int main()
{
    int countN;
    cin >> countN;

    int i = 0;

    while (i < countN)
    {
        int miss = 0;
        int curr = 2;
        int i = 2;
        for (;;)
        {
            i++;
            if (curr % i == 0)
            {
                miss++;
                if (miss < 3) break;
            }
            
        }
        cout << curr;
        curr = curr + 1;
        i++;
    }

}
