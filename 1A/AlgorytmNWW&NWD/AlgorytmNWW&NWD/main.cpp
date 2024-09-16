#include <iostream>
#include <chrono>
#include <unistd.h>
using namespace std;
void nww(int* a, int* b);
void nwd(int* a, int* b);
int main()
{
    short tryb = 0;
    int a,b;
    void (*nwwnwd[])(int*, int*) = { nww, nwd };
    cout << "Wybierz NWW [0] czy NWD [1]: ";
    cin >> tryb;
    tryb = ((tryb == 0) || (tryb == 1))? tryb : 0;
    cout << "Tryb: " << tryb << endl;
    cout << "Pierwsza liczba: "; 
    cin >> a;
    cout << "Druga liczba: "; 
    cin >> b;
    auto start = chrono::steady_clock::now();
    (*nwwnwd[tryb])(&a, &b); 

    auto end = chrono::steady_clock::now();
    cout << chrono::duration_cast<chrono::milliseconds>(end - start).count() << " mill"<< endl;
    return 0;
}

void nww(int* a, int* b)
{
    while(*a != *b) 
        if(*a < *b) *b = *b - *a; else *a = *a - *b;
    cout << "Najwiekszy wspolny dzielnik: " << *a << endl;
}
void nwd(int* a, int* b)
{
    int iloczyn = ((*a) * (*b));
    while(*a != *b)
        if(*a < *b) *b = *b - *a; else *a = *a - *b;
    cout << "Najmniejsza wspolna wielokrotnosc: " << (iloczyn / *a) << endl;
    
}