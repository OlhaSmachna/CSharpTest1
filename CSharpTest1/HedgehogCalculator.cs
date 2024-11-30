using System.Diagnostics;

namespace CSharpTest1
{
    internal class HedgehogCalculator
    {
        private int[] population;
        private int targetColor;

        public HedgehogCalculator(int[] population, int targetColor)
        {
            this.population = population;
            this.targetColor = targetColor;
        }

        public int Calculate()
        {
            // Перевірка на випадок коли всі їжачки відпочатку одного кольору
            if ((population[0] == 0 && population[1] == 0) || (population[0] == 0 && population[2] == 0) || (population[1] == 0 && population[2] == 0))
            {
                if (population[targetColor] == 0)
                    return -1; // Якщо всі їжачки одного кольору і цей колір не відповідає заданому - досягти мети не можливо
                else
                    return 0; // Інакше всі їжачки вже мають бажаний колір, тобто знадобится 0 зустрічей
            }

            /*
             * Для того, щоб визначити мінімальну кількість зустрічей
             * змоделюємо всі можливі варіанти зустричей у вигляді графа,
             * де вершини відображають стан популяції,
             * і виконаємо пошук в ширину -
             * нас цікавить найкоротший шлях до вершини,
             * де population[targetColor]==population.sum(), 
             * а дві інші позиції дорівнюють 0
            */

            // Створюємо пару, ключем якої буде рівень вузла, а значенням - масив із станом популяції
            KeyValuePair<int, int[]> currentState = new KeyValuePair<int, int[]>(0, population);
            // Створюємо чергу відповідного типу для проходу по графу
            Queue<KeyValuePair<int, int[]>> populationState = new Queue<KeyValuePair<int, int[]>>();
            populationState.Enqueue(currentState);
            // Створюємо хешсет для запису пройдених вузлів
            HashSet<int[]> visitedNodes = new HashSet<int[]>();
            bool successFlag = false;
            int maxPopulation = population.Sum();

            while (populationState.Count > 0)
            {
                // Перегляд поточного вузла
                currentState = populationState.Dequeue();
                //Trace.WriteLine(currentState.Value[0] + " " + currentState.Value[1] + " " + currentState.Value[2]);
                // Якщо на ньому виконується умова - вихід з циклу
                if (currentState.Value[targetColor] == maxPopulation)
                {
                    successFlag = true;
                    break;
                }

                // На кожному вузлі можливі 3 розгалуження (для кожного варіанту зустрічі), при умові що ще є їжаки кожного з кольрів
                // змінюємо популяцію для кожного варіанту і додаємо в чергу, але лише за умови, що вузел із такою популяцією ще не було пройдено

                // Нажаль алгоритм вийшов не дуже швидким, тому довелось обмежити розмір графа, обмеживши розмір чисел, що можна вводити в інтерфейсі
                if(!visitedNodes.Where(el => el[0] == currentState.Value[0] && el[1] == currentState.Value[1] && el[2] == currentState.Value[2]).Any())
                {
                    visitedNodes.Add(currentState.Value);

                    int[] temp = [currentState.Value[0] - 1, currentState.Value[1] - 1, currentState.Value[2] + 2];
                    if (temp[0] >= 0 && temp[1] >= 0)
                    {
                        populationState.Enqueue(new KeyValuePair<int, int[]>(currentState.Key + 1, temp));
                    }

                    temp = [currentState.Value[0] - 1, currentState.Value[1] + 2, currentState.Value[2] - 1];
                    if (temp[0] >= 0 && temp[2] >= 0)
                    {
                        populationState.Enqueue(new KeyValuePair<int, int[]>(currentState.Key + 1, temp));
                    }

                    temp = [currentState.Value[0] + 2, currentState.Value[1] - 1, currentState.Value[2] - 1];
                    if (temp[1] >= 0 && temp[2] >= 0)
                    {
                        populationState.Enqueue(new KeyValuePair<int, int[]>(currentState.Key + 1, temp));
                    }
                }
                
            }

            if (successFlag) return currentState.Key;
            else return -1;
        }
    }
}
