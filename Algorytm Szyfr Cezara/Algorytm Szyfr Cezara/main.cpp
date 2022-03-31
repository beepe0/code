#include <iostream>

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
    cin >> word;
    cin >> key;
    key = (key > sizeOfTab || key < -sizeOfTab ) ? 0 : key;
    
    for(int i = 0; i < word.size(); i++)
    {
        for(int k = 0; k < sizeOfTab; k++ )
        {
            if(word[i] == tab[k])
            {
                if((k + key) > 25) answer += tab[(k + key) - sizeOfTab ];
                else if((k + key) < 0) answer += tab[(k + key) + sizeOfTab];
                else answer += tab[(k + key)];
                    
            }
        }
    }
    
    cout << answer;
    

    return 0;
}
