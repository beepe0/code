#include <iostream>

bool isEquals(string word, string nextWord);
int main()
{
    string word, nextWord;
    cin >> word;
    cin >> nextWord;
    if(isEquals(word, nextWord)) std::cout << "To slowo jest jednakowe: " << word << " == " << nextWord;
    else std::cout << "To slowo nie jest jednakowe: " << word << " != " << nextWord;
    return 0;
}

bool isEquals(string word, string nextWord)
{
    int suma = 0;
    if((word.size() != nextWord.size())) return false;
    for(int i = 0; i < word.size(); i++) suma += (int)word[i] - (int)nextWord[i];
    if(suma == 0) return true; else return false;
}