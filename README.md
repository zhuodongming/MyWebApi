# MyWebApi
MyWebApi实现了跨域访问  
实现了Basic身份认证及其授权、异常处理过滤器、Model绑定验证过滤器  
日志使用NLog进行异步记录  
EntityFrameWork底层实现SQL性能监视器：记录EF上下文执行的异常、SqlQuery执行的异常、EF事务执行的异常、执行性能过慢的记录。  
EntityFrameWork底层实现读写分离功能，无需上层应用改动任何现有代码。



