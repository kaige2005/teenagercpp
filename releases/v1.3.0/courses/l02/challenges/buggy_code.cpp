// Bug猎手：找出程序中的逻辑错误

#include <iostream>
using namespace std;

int main() {
    int secret = 42;
    int guess;
    cout << "猜一个数字：";
    cin >> guess;

    // BUG在这里！使用了=而不是==
    if (guess = secret) {
        cout << "猜对了！" << endl;
    } else {
        if (guess < secret) {
            cout << "猜小了！" << endl;
        } else {
            cout << "猜大了！" << endl;
        }
    }

    return 0;
}
