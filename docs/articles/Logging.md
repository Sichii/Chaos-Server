# Logging

## Logging Providers

Chaos utilizes the [NLog logging framework](https://nlog-project.org/) to provide a robust logging system. The default configuration is set
to log only to the console.
This can be changed by modifying the `appsettings.json` file. Continue reading to learn how to enable another built in log target.

Chaos makes great use of object destructuring, combined with NLogs object transformers. If there is a desire to swap the logging provider to
something like [Serilog](https://serilog.net/), you will need to replace this behavior with the `.Destructure.ByTransforming<T>`
feature.

This structured/destructured style of logging data can be more easily viewed via logging tools like [ElasticSearch](https://www.elastic.co/)
or [Seq](https://datalust.co/seq).

## Seq

Seq is by far the simpler of these two tools, and due to this there is a built in way to enable logging to it.  
To quickly and easily enable Seq, set the configuration value `Logging:UseSeq` to `true`, and enter the details of
your Seq instance in the `appsettings.seq.local.json` and `appsettings.seq.prod.json` files. They are preconfigured to use the default
values for a local Seq instance.

## ElasticSearch

While the ELK stack(ElasticSearch, LogStash, Kibana) will be the more familiar tool to experienced developers, it can be difficult for the
less experienced to setup and configure. For this reason, if you want to use the ELK stack in chaos, you will need to set it up yourself.