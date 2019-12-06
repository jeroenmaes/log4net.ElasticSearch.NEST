[![NuGet version (log4net.ElasticSearch.Nest)](https://img.shields.io/nuget/v/log4net.ElasticSearch.Nest.svg?style=flat)](https://www.nuget.org/packages/log4net.ElasticSearch.Nest/)
[![Build Status](https://jeroenmaes.visualstudio.com/Demo/_apis/build/status/JEMS.log4net.ElasticSearch.Nest?branchName=master)](https://jeroenmaes.visualstudio.com/Demo/_build/latest?definitionId=6&branchName=master)
# log4net.ElasticSearch.NEST
log4net.ElasticSearch.NEST is a log4net appender, based on the log4net.ElasticSearch package, for easy logging of exceptions and messages to ElasticSearch indices using the NEST package.

# Usage
```xml
<appender name="myElasticSearchNestAppender" type="log4net.ElasticSearch.NEST.ElasticSearchNestAppender, log4net.ElasticSearch.NEST">
      <layout type="log4net.Layout.PatternLayout,log4net">
        <param name="ConversionPattern" value="%d{ABSOLUTE} %-5p %c{1}:%L - %m%n" />
      </layout>
      <ServerList value="http://localhost:9200/,http://localhost:9200/,http://localhost:9200/"/>
      <IndexName value="demo"/>
      <Rolling value ="true" />
      
      <evaluator type="log4net.Core.LevelEvaluator">
        <threshold value="ALL" />
      </evaluator>
      <bufferSize value="2" />
    </appender>
```
