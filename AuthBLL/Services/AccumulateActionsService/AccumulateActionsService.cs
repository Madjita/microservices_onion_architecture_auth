using System;
using System.Collections.Generic;
using AuthDAL.Models;
using Microsoft.Extensions.Logging;

namespace AuthBLL.Services;

public interface IAccumulateActionsService
{
    void Add(Action action);
    void Invoke();
}

public class AccumulateActionsService : IAccumulateActionsService
{
    private readonly Stack<Action> _actions;
    private readonly ILogger<AccumulateActionsService> _logger;

    public AccumulateActionsService(ILogger<AccumulateActionsService> logger)
    {
        _logger = logger;
        _actions = new Stack<Action>();
    }

    public void Add(Action action)
    {
        _actions.Push(action);
    }

    public void Invoke()
    {
        while (_actions.Count > 0)
            _actions.Pop().Invoke();
    }
}