#pragma once

namespace FDK {
	namespace General {

class FIFO
{
public:
	void 	Clear();						// 初期化
	void	Push( void *obj );				// 格納
	void*	Pop();							// 取り出し
	void*	Peek() {return m_pFirst;}		// 次に取り出す要素を覗き見
	
public:
	FIFO() {m_pFirst=m_pLast=NULL;}

protected:
	struct Cell {
		void*	obj;
		Cell	*prev, *next;
	};
	Cell*	m_pFirst;
	Cell*	m_pLast;
};

	}//General
}//FDK

using namespace FDK::General;
