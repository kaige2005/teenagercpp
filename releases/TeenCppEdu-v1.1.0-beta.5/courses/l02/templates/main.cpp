#include <iostream>
#include <cstdlib>
#include <ctime>
using namespace std;

int main() {
    // 生成随机数（1-100）
    srand(time(0));
    int secret = rand() % 100 + 1;
    int guess;

    cout << "===== 猜数字游戏 =====" << endl;
    cout << "我想了一个1-100之间的数字" << endl;
    cout << "你来猜一猜：";
    cin >> guess;

    // 【任务1】使用 if 判断是否猜对
    // 提示：如果 guess == secret 就猜对了


    // 【任务2】添加 else 分支，给出偏高/偏低提示
    // 提示：使用 > 和 < 来判断


    cout << "游戏结束！" << endl;
    return 0;
}
