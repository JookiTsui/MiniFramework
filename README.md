# MiniFramework
Unity简易开发框架， 相比于其他成熟的框架，更适合新人学习相关功能的实现

## 前言
市面上好用、成熟的框架GitHub上有不少，不过目前阿和用过的只有QFramework, 其他优秀的框架有<br>
• xasset: xasset 致力于为 Unity 项目提供一套精简稳健的资源管理环境 <br>
• TinaX: 开箱即用的 Unity 开发框架，支持 Lua。 <br>
• IFramework（OnClick） Simple Unity Tools <br>
• JEngine JEngine is a streamlined and easy-to-use framework designed for Unity Programmers. <br>
• ET Unity3D Client And C# Server Framework <br>
• LuaProfiler-For-Unity Lua Profiler For Unity支持 XLua、SLua、ToLua <br>
等等， 笔者这里就不一一列举了，有兴趣的可以自己去一一查看。 <br>

既然有这么多优秀的框架为什么阿和还要自己做一个呢？原因很简单：为了学习！ <br>

别人做好的框架拿过来开箱即用，确实能让开发效率大幅提高，平时需要去自己操心的资源加载、内存清理、热更新等内容框架都隐式的帮我们处理好了，我们只需要显示的调用相对应的API。 <br>

省心确实是省心！但是对于一部分有好奇心的开发者来说，一定会去研究背后的实现，因为我们不仅仅满足于只是完成任务，更重要的是提高和丰富自身的技能，为将来的长远做打算。虽然不鼓励大家重复造轮子，但是在自己的专业和研究领域，不说一定要把轮子造出来，但是对于轮子是怎么造出来的这回事势必是要弄清楚。 <br>

既然是学习，那拿别人更加优秀和成熟的框架源码来学习不是更好吗？嗯！这点阿和很赞同，阿和最开始也是这么做的，但是呢，或许是方法不对、亦或者是能力不够，阿和在学习的过程中发现有一个问题，这些框架普遍有一个比较宏大的架构，底层的模块错综复杂，以至于在只是想找某一个功能简单直白的实现要翻来覆去找很久，自身感觉还是比较浪费时间且乏味的。 <br>

至此才冒出了自己要试一试的想法，做一个自己能用且简单直给的框架，并且要给那些富有好奇心的初学者一个更易于学习背后实现的框架，于是MiniFramework诞生了。 <br>

截止目前框架实现是了
• 资源加载
• AssetBundle热更新功能
• csv配置文件加载，支持热更新
• 客户端通用基础网络框架， 支持ProtoBuffer
• 基于枚举的事件机制等

## 文档
https://www.yuque.com/jooki/vwruya
