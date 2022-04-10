#include <SFML/Graphics.hpp>
#include <iostream>
#include <cmath>
class PointsI
{
    public:
    int x = 0;
    int y = 0;

    PointsI(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
};
PointsI neibours[] = 
    {
        {0,0},
        {1,0},
        {-1,0},
        {0,1},
        {0,-1},
        {1,1},
        {1,-1},
        {-1,-1},
        {-1,1}
    };

void GenerateNoise(float **n, PointsI size, int octaves);

int main()
{
    std::cout << "Start"<<std::endl;
    
    PointsI _size = {640,640};
    float **noise;
    noise = new float*[_size.x];
    for(int x = 0; x < _size.x; x++) noise[x] = new float[_size.y];
    GenerateNoise(noise, _size, 50);//octaves 1 .. ~100;

    sf::RenderWindow window(sf::VideoMode(_size.x, _size.y), "Window");
    sf::Vertex point(sf::Vector2f(10, 10), sf::Color::Green);
    
    float sizeUp = 0.3;
    float time = 0;
  
    sf::Clock clock;
    while (window.isOpen())
    {
        sf::Time els = clock.restart();
        std::cout << "Delta:" << els.asSeconds() << std::endl;
        
        time += els.asSeconds();
        sf::Event event;
        while (window.pollEvent(event))
        {
            if (event.type == sf::Event::Closed)
                window.close();
            else if(event.type == sf::Event::MouseWheelMoved)
                sizeUp += float(event.mouseWheel.delta)/200;
        }
        if(time > 0.1f)
        {
            time = 0;
            window.clear();
            for(int x = 0; x < _size.x; x+=1)
            {
                for(int y = 0; y < _size.y; y+=1)
                {
                    sf::Color levels;
                    if(noise[x][y] > sizeUp + 0.02f)
                    {
                        levels.r = 255;
                        levels.g = 255;
                        levels.b = 255;
                        point.position = sf::Vector2f(x,y);
                        point.color = levels;
                        window.draw(&point, 1, sf::Points);
                    }else if(noise[x][y] > sizeUp + 0.01f)
                    {
                        levels.r = 19;
                        levels.g = 134;
                        levels.b = 19;
                        point.position = sf::Vector2f(x,y);
                        point.color = levels;
                        window.draw(&point, 1, sf::Points);
                    }else if(noise[x][y] > sizeUp + 0.002f)
                    {
                        levels.r = 55;
                        levels.g = 202;
                        levels.b = 55;
                        point.position = sf::Vector2f(x,y);
                        point.color = levels;
                        window.draw(&point, 1, sf::Points);
                    }else if(noise[x][y] > sizeUp)
                    {
                        levels.r = 225;
                        levels.g = 240;
                        levels.b = 95;
                        point.position = sf::Vector2f(x,y);
                        point.color = levels;
                        window.draw(&point, 1, sf::Points);
                    }else if(noise[x][y] > sizeUp - 0.006f)
                    {
                        levels.r = 68;
                        levels.g = 199;
                        levels.b = 255;
                        point.position = sf::Vector2f(x,y);
                        point.color = levels;
                        window.draw(&point, 1, sf::Points);
                    }else if(noise[x][y] > sizeUp - 0.015f)
                    {
                        levels.r = 17;
                        levels.g = 125;
                        levels.b = 171;
                        point.position = sf::Vector2f(x,y);
                        point.color = levels;
                        window.draw(&point, 1, sf::Points);
                    }
                    else 
                    {
                        levels.r = 0;
                        levels.g = 79;
                        levels.b = 112;
                        point.position = sf::Vector2f(x,y);
                        point.color = levels;
                        window.draw(&point, 1, sf::Points);
                    }
                }
            }
            window.display();
        }
        
    }

    return 0;
}

void GenerateNoise(float **n, PointsI size, int octaves)
{
    
    for(int x = 0; x < size.x; x++)
        for(int y = 0; y < size.y; y++)
            n[x][y] = float(rand()%100)/100;

    float equal = 0;
    int curr = 0;
    int length = (sizeof(neibours) / sizeof(neibours[0]));

    for(int o = 0; o < octaves; o++)
    for(int x = 0; x < size.x; x++)
    {
        for(int y = 0; y < size.y; y++)
        {
            for(int i = 0; i < length; i++)
            {
                PointsI p = neibours[i];
                PointsI _neibours = {(x + p.x), (y + p.y)};
                if((_neibours.x > 0 && _neibours.y > 0) && (_neibours.x < size.x && _neibours.y < size.y))
                {
                    curr++;
                    equal += n[_neibours.x][_neibours.y];
                }
            }
            n[x][y] = float(equal / curr);
            curr = 0;
            equal = 0;
        }

    }
}