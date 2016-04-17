# MyWebApi
 WebApi实现了跨域、日志使用NLog进行异步记录
 使用EntityFrameWork，并添加了底层SQL性能监视：记录EF上下文执行的异常、SqlQuery执行的异常、EF事务执行的异常。当EF上下文生成的Sql语句或者SqlQuery的执行时间超过某个配置值(如50毫秒)就进行记录。


