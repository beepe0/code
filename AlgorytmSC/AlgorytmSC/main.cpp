#include <iostream>
#include <chrono>
#include <unistd.h>
using namespace std;

int main()
{
    string word, answer;
    short key;
    
    char tab[] =
    {
        'a','b','c','d','e',
        'f','g','h','i','j',
        'k','l','m','n','o',
        'p','q','r','s','t',
        'u','v','w','x','y',
        'z'
    };
    short sizeOfTab = sizeof(tab) / sizeof(tab[0]);
    cout << "Podaj text: ";
    getline(cin, word);
    cout << "Podaj klucz: ";
    cin >> key;
    auto start = chrono::steady_clock::now();
    key = (key > sizeOfTab || key < -sizeOfTab ) ? 0 : key;
    for(int i = 0; i < word.size(); i++)
        for(int k = 0; k < sizeOfTab; k++ )
            if(word[i] == tab[k])
            {
                if((k + key) > 25) answer += tab[(k + key) - sizeOfTab ];
                else if((k + key) < 0) answer += tab[(k + key) + sizeOfTab];
                else answer += tab[(k + key)];
            }else if(word[i] == ' '){answer += word[i]; break;}
    cout << "Wynik: "<< answer << endl;
    auto end = chrono::steady_clock::now();
    cout << chrono::duration_cast<chrono::milliseconds>(end - start).count() << " mill\n";
    return 0;
}
