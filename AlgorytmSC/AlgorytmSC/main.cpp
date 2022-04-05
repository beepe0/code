#include <iostream>
#include <chrono>
#include <unistd.h>
using namespace std;

void smallL(string word, string* answer, short key, int curr);
void hugeL(string word, string* answer, short key, int curr);

char tab[] =
    {
        'a','b','c','d','e',
        'f','g','h','i','j',
        'k','l','m','n','o',
        'p','q','r','s','t',
        'u','v','w','x','y',
        'z'
    };
char tabUpper[] =
    {
        'A','B','B','D','E',
        'F','G','H','I','J',
        'K','L','M','N','O',
        'P','Q','R','S','T',
        'U','V','W','X','Y',
        'Z'
    };
short sizeOfTab = sizeof(tab) / sizeof(tab[0]);

int main()
{
    string word, answer;
    short key;
    
    cout << "Podaj text: ";
    getline(cin, word);
    cout << "Podaj klucz: ";
    cin >> key;
    auto start = chrono::steady_clock::now();
    key = (key > sizeOfTab || key < -sizeOfTab ) ? 0 : key;
    
    for(int i = 0; i < word.size(); i++)
        if(isupper(word[i])) hugeL(word, &answer, key, i);
        else smallL(word, &answer, key, i);

    cout << "Wynik: "<< answer << endl;
    auto end = chrono::steady_clock::now();
    cout << chrono::duration_cast<chrono::milliseconds>(end - start).count() << " mill\n";
    return 0;
}
void smallL(string word, string* answer, short key, int curr)
{
	for(int k = 0; k < sizeOfTab; k++)
	{
		if(word[curr] == tab[k])
		{
			if((k + key) > 25) *answer += tab[(k + key) - sizeOfTab ];
			else if((k + key) < 0) *answer += tab[(k + key) + sizeOfTab];
			else *answer += tab[(k + key)];
			
		}else if(word[curr] == ' '){*answer += word[curr]; break;}
		
	}
}

void hugeL(string word, string* answer, short key, int curr)
{
	for(int k = 0; k < sizeOfTab; k++)
	{
		if(word[curr] == tabUpper[k])
		{
			if((k + key) > 25) *answer += tabUpper[(k + key) - sizeOfTab ];
			else if((k + key) < 0) *answer += tabUpper[(k + key) + sizeOfTab];
			else *answer += tabUpper[(k + key)];
		}
	}
}

