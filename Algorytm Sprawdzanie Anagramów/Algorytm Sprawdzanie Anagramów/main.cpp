#include <iostream>

bool isEquals(std::string word, std::string nextWord);
int main()
{
    std::string word, nextWord;
    std::cout << "Pierwsze słowo: " << std::endl;
    std::cin >> word;
    std::cout << "Drugie słowo: " << std::endl;
    std::cin >> nextWord;
    if(isEquals(word, nextWord)) std::cout << "To slowo jest jednakowe: " << word << " == " << nextWord;
    else std::cout << "To slowo nie jest jednakowe: " << word << " != " << nextWord;
    return 0;
}



bool isEquals(std::string word, std::string nextWord)
{
    int suma = 0;
    if((word.size() != nextWord.size())) return false;
    for(int i = 0; i < word.size(); i++) suma += (int)word[i] - (int)nextWord[i];
    if(suma == 0) return true; else return false;
}