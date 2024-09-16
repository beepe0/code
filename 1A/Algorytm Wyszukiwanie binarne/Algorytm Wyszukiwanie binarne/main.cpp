#include <iostream>

using namespace std;

int FL(int liczba, int* tab, int L, int P);

int main()
{
    int tab[] = {2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47};
    cout << "Liczba " << FL(43, tab,0,15)<<endl;
    return 0;
}

int FL(int liczba, int* tab, int L, int P)
{
    int l = L, p = P, sr = (l+p)/2; 
    while(l <= p)
    {
        if(tab[sr] == liczba) return sr;
        if(tab[sr] > liczba) p = sr - 1;
        else l = sr + 1;
        sr = (l+p)/2;
        
    }
    return -1;
}
